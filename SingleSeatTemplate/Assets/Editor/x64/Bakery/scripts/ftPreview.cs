#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Threading;

/*
Not supported by RTPreview:
- Resolution settings (except UVGBuffer)
- Scenes not fitting in VRAM together with Unity
- Sample position adjustment
- Baked normal maps
- Post-processing (image is LDR)
- Cloud shadows
- Bitmasks
- Moving geometry at runtime (including light meshes)
- Toggling geometry at runtime
- LMGroup Transparent selfshadow feature
- SSS
- AO

TODO:
- rtSceneViewPreviewOn?
- possibly wrong Z rotation of IES (both preview and batch points)
- Test UPR/HDRP
- Update scene
*/

public class ftPreview : EditorWindow
{
    [DllImport ("frender", CallingConvention=CallingConvention.Cdecl)]
    public static extern bool StartPreviewInput();

    [DllImport ("frender", CallingConvention=CallingConvention.Cdecl)]
    public static extern bool SendPreviewInput(float rx, float ry, float rz,
                                               float ux, float uy, float uz,
                                               float fx, float fy, float fz,
                                               float px, float py, float pz,
                                               float fov, float exposure, float backFaceWeight, float emissiveBoost,
                                               int refreshConstVer, int refreshFullVer, int frameVer, int maxIntegrationSteps);

    [DllImport ("frender", CallingConvention=CallingConvention.Cdecl)]
    public static extern void EndPreviewInput();

    [DllImport ("frender", CallingConvention=CallingConvention.Cdecl)]
    public static extern bool SendPreviewData(byte[] data, int dataSize);

    [DllImport ("frender", CallingConvention=CallingConvention.Cdecl)]
    public static extern bool GetPreviewImage(byte[] data, int dataSize);

    public static int pwidth = 640;
    public static int pheight = 360;
    public static int psteps = 8192;
    public static bool exportScene = true;
    public static bool exportLights = true;
    public static bool renderInSceneView = false;

    static IEnumerator progressFunc;
    static System.IntPtr pprocess = (System.IntPtr)0;
    static bool inputActive = false;

    public static ftPreview Active;
    static int refresh = 0;
    static int refreshConstVer = 0;
    static int refreshFullVer = 0;
    static int frameVer = 0;
    static int waitForFrame = 0;

    const int PREVIEW_MAXDATASIZE = 4096 * 1024;
    const int PREVIEW_HISTORY = 100;

    static List<BakeryDirectLight> allDirects;
    static List<BakeryPointLight> allPoints;
    static List<BakerySkyLight> allSkies;

    static float lastIndirectBoost, lastBackFaceWeight, lastEmissiveBoost;
    static int lastBounces;
    static float exposure = 1.0f;
    static bool origViewportRefresh;
    static bool exiting = false;

    static MemoryStream mmStream = null;
    static BinaryWriter mmWriter = null;
    static Mutex mutex;
    static SceneView sceneView = null;
    static ftLightmapsStorage renderSettingsStorage;

    static byte[] previewImageBuffer = null;
    static Texture2D previewImageTex = null;
    static Matrix4x4[] viewProjHistory;
    static int historyHead = 0;

    static Matrix4x4 camViewProj;
    static Vector3 camRht, camUp, camFwd, camPos;
    static float camFov;

    static Shader previewShader;
    public static bool enabled;

    static void StopPreview()
    {
        EndPreviewInput();
        if (mmStream != null)
        {
            mmStream.Close();
            mmStream = null;
        }
        if (mmWriter != null) mmWriter = null;
        if (mutex != null)
        {
            //Debug.LogError("Mutex removing 2");
            mutex.ReleaseMutex();
            mutex = null;
        }
        inputActive = false;
        EditorApplication.update -= UpdateInput;
        EditorSceneManager.sceneOpened -= OnSceneOpen;
        if (Active != null) Active.Repaint();
        ToggleSceneView(false);
        exiting = false;
        EditorUtility.UnloadUnusedAssetsImmediate();

        if (exportScene) ftLightmaps.RefreshFull();
    }

    static void OnSceneOpen(Scene scene, OpenSceneMode mode)
    {
        bool previewActive = (pprocess != (System.IntPtr)0 && !ftRenderLightmap.IsProcessFinished(pprocess));
        Debug.LogWarning(previewActive);
        if (previewActive)
        {
            if (!SendPreviewInput(0,0,0,
                                  0,0,0,
                                  0,0,0,
                                  0,0,0,
                                  -1, 0, 0, 0,
                                  0,0,0,1))
            {
                Debug.LogError("Failed to send input to preview (2)");
            }
            StopPreview();
        }
    }

    static void WriteNullTerminatedStringWithNewLine(BinaryWriter f, string s)
    {
        for(int i=0; i<s.Length; i++) f.Write(s[i]);
        f.Write((byte)0);
        f.Write((byte)'\n');
    }

    public static bool IsActive()
    {
        if (pprocess != (System.IntPtr)0 && !ftRenderLightmap.IsProcessFinished(pprocess))
        {
            return true;
        }
        return false;
    }

    static public void GeneratePreviewSettings(bool fast, bool mmap)
    {
        // Collect lights
        allDirects = new List<BakeryDirectLight>();
        var _allDirects = FindObjectsOfType(typeof(BakeryDirectLight)) as BakeryDirectLight[];
        for(int i=0; i<_allDirects.Length; i++)
        {
            var obj = _allDirects[i] as BakeryDirectLight;
            if (!obj.enabled) continue;
            allDirects.Add(obj);
        }
        allSkies = new List<BakerySkyLight>();
        var _allSkies = FindObjectsOfType(typeof(BakerySkyLight)) as BakerySkyLight[];
        for(int i=0; i<_allSkies.Length; i++)
        {
            var obj = _allSkies[i] as BakerySkyLight;
            if (!obj.enabled) continue;
            allSkies.Add(obj);
        }
        allPoints = new List<BakeryPointLight>();
        var _allPoints = FindObjectsOfType(typeof(BakeryPointLight)) as BakeryPointLight[];
        for(int i=0; i<_allPoints.Length; i++)
        {
            var obj = _allPoints[i] as BakeryPointLight;
            if (!obj.enabled) continue;
            allPoints.Add(obj);
        }
        var allAreas = new List<BakeryLightMesh>();
        var smallAreas = new List<BakeryLightMesh>();
        var smallAreasBuffers = new List<List<Vector3>>();
        var smallAreasPointSize = new List<float>();
        int totalVPLCount = 0;
        var _allAreas = FindObjectsOfType(typeof(BakeryLightMesh)) as BakeryLightMesh[];
        const int sampleLimit = 0; // compare // disable for now
        const int sampleForceLimit = 64; // set

        for(int i=0; i<allDirects.Count; i++)
        {
            allDirects[i].transform.hasChanged = false;
        }
        for(int i=0; i<allPoints.Count; i++)
        {
            allPoints[i].transform.hasChanged = false;
        }
        for(int i=0; i<allSkies.Count; i++)
        {
            allSkies[i].transform.hasChanged = false;
        }

        lastIndirectBoost = ftRenderLightmap.hackIndirectBoost;
        lastBackFaceWeight = ftRenderLightmap.giBackFaceWeight;
        lastEmissiveBoost = 1.0f;//ftRenderLightmap.hackEmissiveBoost;
        lastBounces = ftRenderLightmap.bounces;

        if (renderInSceneView)
        {
            var sceneViewRect = sceneView.camera.pixelRect;
            pwidth = (int)sceneViewRect.width;
            pheight = (int)sceneViewRect.height;
        }

        for(int i=0; i<_allAreas.Length; i++)
        {
            var obj = _allAreas[i] as BakeryLightMesh;
            if (!obj.enabled) continue;

            while(allAreas.Count <= -obj.lmid) allAreas.Add(null);
            allAreas[-obj.lmid] = obj;

            //Debug.LogError("Set " +obj.name+" "+obj.lmid);

            if (!fast)
            {
                List<Vector3> l = null;
                if (obj.samples <= sampleLimit) l = new List<Vector3>();
                Vector3[] corners = null;
                var lma = obj.GetComponent<Light>();
                if (lma != null && ftLightMeshInspector.IsArea(lma))
                {
                    corners = ftLightMeshInspector.GetAreaLightCorners(lma);
                }
                float lightPointSize = ftBuildLights.BuildLight(obj, System.Math.Min(obj.samples, sampleForceLimit), corners, "light" + i + ".bin", l);
                if (l != null)
                {
                    smallAreas.Add(obj);
                    smallAreasBuffers.Add(l);
                    smallAreasPointSize.Add(lightPointSize);
                    totalVPLCount += l.Count / 3;
                }
            }
        }
        //Debug.LogError(allAreas.Count);

        // Write lights
        BinaryWriter flights = null;
        if (!mmap)
        {
            flights = new BinaryWriter(File.Open(ftRenderLightmap.scenePath + "/preview.bin", FileMode.Create));
        }
        else
        {
            if (mmStream == null)
            {
                var mmArray = new byte[PREVIEW_MAXDATASIZE];
                mmStream = new MemoryStream(mmArray);
                mmWriter = new BinaryWriter(mmStream);
            }
            mmStream.Seek(0, SeekOrigin.Begin);
            flights = mmWriter;
        }

        bool mutexCreated;
        mutex = new Mutex(true, "FTPREVIEWMUTEX", out mutexCreated);
        //Debug.LogError("Mutex created " + mutexCreated);
        if (!mutexCreated)
        {
            mutex.WaitOne();
        }

        int previewBounces = ftRenderLightmap.bounces + 1;
        flights.Write(pwidth);
        flights.Write(pheight);
        flights.Write(previewBounces);
        flights.Write(renderInSceneView ? (int)1 : (int)0);
        int numGlobalLights = allDirects.Count + allSkies.Count;
        int numLocalLights = allPoints.Count + totalVPLCount;
        int numAreas = allAreas.Count;
        flights.Write(numGlobalLights);
        flights.Write(numLocalLights);
        flights.Write(numAreas);
        for(int i=0; i<allDirects.Count; i++)
        {
            var tform = allDirects[i].transform;
            flights.Write(Mathf.Min((allDirects[i].indirectIntensity * ftRenderLightmap.hackIndirectBoost) / 100.0f, 0.99f));
            flights.Write(tform.forward.x);
            flights.Write(tform.forward.y);
            flights.Write(tform.forward.z);
            flights.Write(allDirects[i].samples == 0 ? -1 : allDirects[i].shadowSpread);
            flights.Write(allDirects[i].color.linear.r * allDirects[i].intensity);
            flights.Write(allDirects[i].color.linear.g * allDirects[i].intensity);
            flights.Write(allDirects[i].color.linear.b * allDirects[i].intensity);
        }
        for(int i=0; i<allSkies.Count; i++)
        {
            flights.Write(1.0f + Mathf.Min((allSkies[i].indirectIntensity * ftRenderLightmap.hackIndirectBoost) / 100.0f, 0.99f));
            flights.Write(allSkies[i].cubemap ? 1.0f : 0.0f);
            flights.Write(0.0f);
            flights.Write(0.0f);
            flights.Write(allSkies[i].hemispherical ? 1.0f : 0.0f);
            flights.Write(allSkies[i].color.linear.r * allSkies[i].intensity);
            flights.Write(allSkies[i].color.linear.g * allSkies[i].intensity);
            flights.Write(allSkies[i].color.linear.b * allSkies[i].intensity);
            if (!fast)
            {
                ftBuildLights.BuildSkyLight(allSkies[i], 1, allSkies[i].cubemap ? true : false, "sky.bin");
            }
        }
        for(int i=0; i<allPoints.Count; i++)
        {
            var tform = allPoints[i].transform;
            var projMode = allPoints[i].projMode;

            var obj = allPoints[i];
            if (obj.projMode == BakeryPointLight.ftLightProjectionMode.Cookie && obj.cookie == null) projMode = BakeryPointLight.ftLightProjectionMode.Omni;
            if (obj.projMode == BakeryPointLight.ftLightProjectionMode.Cubemap && obj.cubemap == null) projMode = BakeryPointLight.ftLightProjectionMode.Omni;
            if (obj.projMode == BakeryPointLight.ftLightProjectionMode.IES && obj.iesFile == null) projMode = BakeryPointLight.ftLightProjectionMode.Omni;

            float projParam1 = 0.0f;
            float projParam2 = 0.0f;
            if (projMode == BakeryPointLight.ftLightProjectionMode.Cone)
            {
                projParam1 = allPoints[i].angle;
                projParam2 = allPoints[i].innerAngle / 100.0f;
            }
            else if (projMode == BakeryPointLight.ftLightProjectionMode.IES)
            {
                projMode = BakeryPointLight.ftLightProjectionMode.Cubemap;
            }
            else if (projMode == BakeryPointLight.ftLightProjectionMode.Cookie)
            {
                projParam2 = allPoints[i].angle;
            }

            flights.Write((float)projMode);
            flights.Write(tform.position.x);
            flights.Write(tform.position.y);
            flights.Write(tform.position.z);

            flights.Write(allPoints[i].shadowSpread);
            flights.Write(allPoints[i].realisticFalloff ? 1.0f : ((1.0f / allPoints[i].cutoff) * 5.0f));
            flights.Write(allPoints[i].realisticFalloff ? (allPoints[i].falloffMinRadius * allPoints[i].falloffMinRadius) : 1.0f);
            flights.Write(1.0f / allPoints[i].cutoff);

            flights.Write(allPoints[i].color.linear.r * allPoints[i].intensity);
            flights.Write(allPoints[i].color.linear.g * allPoints[i].intensity);
            flights.Write(allPoints[i].color.linear.b * allPoints[i].intensity);
            flights.Write(projParam1);

            flights.Write(tform.right.x);
            flights.Write(tform.right.y);
            flights.Write(tform.right.z);
            flights.Write(projParam2);

            flights.Write(tform.up.x);
            flights.Write(tform.up.y);
            flights.Write(tform.up.z);
            flights.Write(allPoints[i].indirectIntensity * ftRenderLightmap.hackIndirectBoost);

            flights.Write(tform.forward.x);
            flights.Write(tform.forward.y);
            flights.Write(tform.forward.z);
            flights.Write(allPoints[i].samples > 0 ? 1.0f : 0.0f);

            if (!fast)
            {
                if (projMode == BakeryPointLight.ftLightProjectionMode.Cubemap || projMode == BakeryPointLight.ftLightProjectionMode.IES ||
                    projMode == BakeryPointLight.ftLightProjectionMode.Cookie)
                {
                    ftBuildLights.BuildLight(allPoints[i], 1, true, true);
                }
            }
        }
        for(int i=0; i<smallAreas.Count; i++)
        {
            var buff = smallAreasBuffers[i];
            float hemisphereArea = 2 * 3.141592653589793f;
            float m = smallAreasPointSize[i] / hemisphereArea;
            m *= 2;
            m /= buff.Count/3;
            var baseR = (smallAreas[i].color.linear.r * smallAreas[i].intensity) * m;
            var baseG = (smallAreas[i].color.linear.g * smallAreas[i].intensity) * m;
            var baseB = (smallAreas[i].color.linear.b * smallAreas[i].intensity) * m;
            for(int j=0; j<buff.Count; j+=3)
            {
                flights.Write(4.0f);
                flights.Write(buff[j].x);
                flights.Write(buff[j].y);
                flights.Write(buff[j].z);

                flights.Write(0.0f);
                flights.Write(1.0f);
                flights.Write(smallAreasPointSize[i]);
                flights.Write(1.0f / smallAreas[i].cutoff);

                flights.Write(baseR * buff[j+2].x);
                flights.Write(baseG * buff[j+2].y);
                flights.Write(baseB * buff[j+2].z);
                flights.Write(180.0f);

                flights.Write(1.0f);
                flights.Write(0.0f);
                flights.Write(0.0f);
                flights.Write(0.0f);

                flights.Write(0.0f);
                flights.Write(1.0f);
                flights.Write(0.0f);
                flights.Write(0.0f);

                flights.Write(buff[j+1].x);
                flights.Write(buff[j+1].y);
                flights.Write(buff[j+1].z);
                flights.Write(0.0f);
            }
        }
        for(int i=0; i<allAreas.Count; i++)
        {
            if (allAreas[i] == null || allAreas[i].samples <= sampleLimit)
            {
                flights.Write(0.0f);
                flights.Write(0.0f);
                flights.Write(0.0f);
                flights.Write(0.0f);
            }
            else
            {
                var color = allAreas[i].color.linear;
                int val1 = ((int)(color.r*255) << 16) | ((int)(color.g*255) << 8) | ((int)(color.b*255));
                float val2 = allAreas[i].intensity;
                float val3 = allAreas[i].indirectIntensity * ftRenderLightmap.hackIndirectBoost;
                float val4 = (allAreas[i].texture != null ? -1.0f : 1.0f) / allAreas[i].cutoff;
                if (!allAreas[i].selfShadow)
                {
                    val3 = (val3==0.0f ? -float.Epsilon : -val3);
                }
                flights.Write(val1);
                flights.Write(val2);
                flights.Write(val3);
                flights.Write(val4);
            }
        }
        for(int i=0; i<allSkies.Count; i++)
        {
            if (allSkies[i].cubemap)
            {
                WriteNullTerminatedStringWithNewLine(flights, "sky" + allSkies[i].UID + ".dds");
            }
        }
        for(int i=0; i<allPoints.Count; i++)
        {
            var projMode = allPoints[i].projMode;

            var obj = allPoints[i];
            if (obj.projMode == BakeryPointLight.ftLightProjectionMode.Cookie && obj.cookie == null) projMode = BakeryPointLight.ftLightProjectionMode.Omni;
            if (obj.projMode == BakeryPointLight.ftLightProjectionMode.Cubemap && obj.cubemap == null) projMode = BakeryPointLight.ftLightProjectionMode.Omni;
            if (obj.projMode == BakeryPointLight.ftLightProjectionMode.IES && obj.iesFile == null) projMode = BakeryPointLight.ftLightProjectionMode.Omni;

            if (projMode == BakeryPointLight.ftLightProjectionMode.Cubemap)
            {
                WriteNullTerminatedStringWithNewLine(flights, ftBuildLights.GetTempTexName(allPoints[i].cubemap));
            }
            else if (projMode == BakeryPointLight.ftLightProjectionMode.IES)
            {
                WriteNullTerminatedStringWithNewLine(flights, ftBuildLights.GetTempTexName(allPoints[i].iesFile));
            }
            else if (projMode == BakeryPointLight.ftLightProjectionMode.Cookie)
            {
                WriteNullTerminatedStringWithNewLine(flights, ftBuildLights.GetTempTexName(allPoints[i].cookie));
            }
        }
        for(int i=0; i<allAreas.Count; i++)
        {
            if (allAreas[i] != null && allAreas[i].texture != null && allAreas[i].samples > sampleLimit)
            {
                WriteNullTerminatedStringWithNewLine(flights, ftBuildLights.GetTempTexName(allAreas[i].texture, "areatex_"));
            }
        }

        if (!mmap)
        {
            flights.Close();
        }
        else
        {
            var data = mmStream.ToArray();
            if (!SendPreviewData(data, (int)mmStream.Position))
            {
                Debug.LogError("Failed to send data to preview");
                StopPreview();
            }
            //Debug.LogError("Sent " + mmStream.Position);
        }

        //Debug.LogError("Mutex removing");
        mutex.ReleaseMutex();
        mutex = null;
        EditorUtility.UnloadUnusedAssetsImmediate();
    }

    void OnGUI()
    {
        int y = 0;

        bool previewActive = (pprocess != (System.IntPtr)0 && !ftRenderLightmap.IsProcessFinished(pprocess));

        if (previewActive || renderInSceneView) GUI.enabled = false;


        var numberBoxStyle = EditorStyles.numberField;
#if UNITY_2019_3_OR_NEWER
        numberBoxStyle = new GUIStyle(numberBoxStyle);
        numberBoxStyle.alignment = TextAnchor.MiddleLeft;
        numberBoxStyle.contentOffset = new Vector2(0, -1);
#endif

        GUI.Label(new Rect(10, y, 100, 15), new GUIContent("Width:", "Preview window width"));
        pwidth = EditorGUI.IntField(new Rect(110, y, 110, 15), pwidth, numberBoxStyle);
        if (pwidth < 1) pwidth = 1;
        if (pwidth > 4096) pwidth = 4096;
        y += 20;

        GUI.Label(new Rect(10, y, 100, 15), new GUIContent("Height:", "Preview window height"));
        pheight = EditorGUI.IntField(new Rect(110, y, 110, 15), pheight, numberBoxStyle);
        if (pheight < 1) pheight = 1;
        if (pheight > 4096) pheight = 4096;
        y += 20;

        if (previewActive || renderInSceneView) GUI.enabled = true;

        GUI.Label(new Rect(10, y, 100, 15), new GUIContent("Iteration limit:", "How many frames should RTPreview accumulate before stopping"));
        psteps = EditorGUI.IntField(new Rect(110, y, 110, 15), psteps, numberBoxStyle);
        if (pheight < 1) pheight = 1;
        if (pheight > 8192) pheight = 8192;
        y += 20;

        if (!previewActive)
        {
            exportScene = GUI.Toggle(new Rect(10, y, 200, 20), exportScene, new GUIContent("Export geometry and maps", "Exports geometry, textures and lightmap properties to Bakery format. This is required, but if you already rendered the scene, and if no changes to meshes/maps/lightmap resolution took place, you may disable this checkbox to skip this step."));
            y += 20;

            //exportLights = GUI.Toggle(new Rect(10, y, 200, 20), exportLights, new GUIContent("Export light textures", "Exports HDRIs and cookies."));
            //y += 20;

            renderInSceneView = GUI.Toggle(new Rect(10, y, 200, 20), renderInSceneView, new GUIContent("Render in Scene View", "Send ray-traced image to Unity and visualize in Scene View. Otherwise, render in a dedicated window."));
            y += 20;
        }

        if (GUI.Button(new Rect(10, y, 230, 30), previewActive ? "Close Preview" : "Open Preview"))
        {
            if (!previewActive)
            {
                ftRenderLightmap bakery = ftRenderLightmap.instance != null ? ftRenderLightmap.instance : new ftRenderLightmap();
                bakery.LoadRenderSettings();

                progressFunc = StartPreviewFunc();
                EditorApplication.update += StartPreview;
            }
            else
            {
                exiting = true;
            }
        }
        y += 30;

        string msg = "Preview is inactive";
        if (previewActive)
        {
            msg = "Preview is active";
        }
        EditorGUI.HelpBox(new Rect(15,y+5,220,40), msg, MessageType.Info);
        y += 40;

        if (previewActive)
        {
            y += 15;
            var textBoxStyle = EditorStyles.textField;
#if UNITY_2019_3_OR_NEWER
            textBoxStyle = new GUIStyle(textBoxStyle);
            textBoxStyle.alignment = TextAnchor.MiddleLeft;
            textBoxStyle.contentOffset = new Vector2(0, -1);
#endif
            GUI.Label(new Rect(10, y, 70, 15), "Exposure:");
            float prev = exposure;
            exposure = EditorGUI.FloatField(new Rect(70, y, 45, 15), exposure, textBoxStyle);
            bool textEdited = (prev != exposure);
            float maxExp = 20;
            float expPow = 2.0f;
            float exposureSlider = Mathf.Pow(exposure/maxExp, 1.0f/expPow);
            exposureSlider = GUI.HorizontalSlider(new Rect(125, y, 120-25, 15), exposureSlider, 0.0f, 1.0f);
            prev = exposure;
            exposure = (Mathf.Pow(exposureSlider, expPow)) * maxExp;
            if (prev != exposure && !textEdited)
            {
                GUI.FocusControl(null); // fix input field focus
            }
            y += 20;
        }

        this.minSize = new Vector2(250, y + 10);
        this.maxSize = new Vector2(this.minSize.x, this.minSize.y + 1);

        if (!previewActive) SaveRenderSettings();
    }

    public void SaveRenderSettings()
    {
        if (renderSettingsStorage == null) renderSettingsStorage = ftRenderLightmap.FindRenderSettingsStorage();
        if (renderSettingsStorage != null)
        {
            if (renderSettingsStorage.renderSettingsRTPVExport != exportScene ||
                renderSettingsStorage.renderSettingsRTPVSceneView != renderInSceneView ||
                renderSettingsStorage.renderSettingsRTPVWidth != pwidth ||
                renderSettingsStorage.renderSettingsRTPVHeight != pheight)
            {
                Undo.RecordObject(renderSettingsStorage, "Change Bakery RTPreview settings");
                renderSettingsStorage.renderSettingsRTPVExport = exportScene;
                renderSettingsStorage.renderSettingsRTPVSceneView = renderInSceneView;
                renderSettingsStorage.renderSettingsRTPVWidth = pwidth;
                renderSettingsStorage.renderSettingsRTPVHeight = pheight;
            }
        }
    }

    void OnEnable()
    {
        LoadRenderSettings();
    }

    public void LoadRenderSettings()
    {
        if (renderSettingsStorage == null) renderSettingsStorage = ftRenderLightmap.FindRenderSettingsStorage();
        if (renderSettingsStorage != null)
        {
            exportScene = renderSettingsStorage.renderSettingsRTPVExport;
            renderInSceneView = renderSettingsStorage.renderSettingsRTPVSceneView;
            pwidth = renderSettingsStorage.renderSettingsRTPVWidth;
            pheight = renderSettingsStorage.renderSettingsRTPVHeight;
        }
    }

    static bool ForceViewportRefresh(bool newVal)
    {
        var viewStateT = sceneView.GetType().GetField("m_SceneViewState", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (viewStateT != null)
        {
            var viewState = viewStateT.GetValue(sceneView);
            if (viewState != null)
            {
                var animVal = viewState.GetType().GetField("showMaterialUpdate");
                if (animVal != null)
                {
                    bool oldVal = (bool)animVal.GetValue(viewState);
                    animVal.SetValue(viewState, newVal);
                    return oldVal;
                }
            }
        }
        return false;
    }

    public static void ToggleSceneView(bool mode)
    {
        var sceneView = SceneView.lastActiveSceneView;
        if (sceneView == null)
        {
            Debug.LogError("Can't get SceneView");
            return;
        }
        if (!mode && enabled)
        {
            ForceViewportRefresh(origViewportRefresh);

            sceneView.SetSceneViewShaderReplace(null, null);
            enabled = false;

            var gstorage = ftLightmaps.GetGlobalStorage();
            gstorage.rtSceneViewPreviewOn = false;
            EditorUtility.SetDirty(gstorage);
        }
        else if (mode)
        {
            //if (checkerShader == null)
            {
                previewShader = Shader.Find("Hidden/ftShowPreview");
                if (previewShader == null)
                {
                    Debug.LogError("Can't load preview rendering shader");
                    return;
                }
            }

            // Force scene view real-time refresh
            origViewportRefresh = ForceViewportRefresh(true);

            sceneView.SetSceneViewShaderReplace(previewShader, null);
            enabled = true;

            var gstorage = ftLightmaps.GetGlobalStorage();
            gstorage.rtSceneViewPreviewOn = true;
            EditorUtility.SetDirty(gstorage);
        }
        sceneView.Repaint();
    }

    static void InitPreviewImage()
    {
        var sceneViewRect = sceneView.camera.pixelRect;
        pwidth = (int)sceneViewRect.width;
        pheight = (int)sceneViewRect.height;

        if (previewImageTex != null)
        {
            previewImageTex.Reinitialize(pwidth, pheight);
            previewImageTex.Apply();
        }
        else
        {
            previewImageTex = new Texture2D(pwidth, pheight, TextureFormat.RGBA32, false, false);
            previewImageTex.wrapMode = TextureWrapMode.Clamp;
        }

        previewImageBuffer = new byte[pwidth * pheight * 4 + 4]; // buffer + frame num
        Shader.SetGlobalTexture("_ftPreviewTexture", previewImageTex);
    }

    IEnumerator StartPreviewFunc()
    {
        refresh = 0;
        refreshConstVer = 0;
        refreshFullVer = 0;
        frameVer = 0;
        waitForFrame = 0;

        if (renderInSceneView)
        {
            if (sceneView == null) sceneView = GetWindow<SceneView>();
            InitPreviewImage();

            viewProjHistory = new Matrix4x4[PREVIEW_HISTORY];
            historyHead = 0;

            var renderSettingsStorage = ftRenderLightmap.FindRenderSettingsStorage();
            renderSettingsStorage.debugTex = previewImageTex;
        }

        BakeryPointLight.lightsChanged = 0;
        BakeryDirectLight.lightsChanged = 0;
        BakerySkyLight.lightsChanged = 0;
        BakeryLightMesh.lightsChanged = 0;
        refreshConstVer = 0;
        refreshFullVer = 0;
        frameVer = 0;
        if (exportScene)
        {
            ftBuildGraphics.modifyLightmapStorage = false;
            ftBuildGraphics.forceAllAreaLightsSelfshadow = true;
            ftBuildGraphics.validateLightmapStorageImmutability = false;
            ftRenderLightmap.hasAnyProbes = false;
            ftRenderLightmap.hasAnyVolumes = false;
            var exportSceneFunc = ftBuildGraphics.ExportScene((ftRenderLightmap)EditorWindow.GetWindow(typeof(ftRenderLightmap)), true);
            ftRenderLightmap.progressBarEnabled = true;
            while(exportSceneFunc.MoveNext())
            {
                ftRenderLightmap.progressBarText = ftBuildGraphics.progressBarText;
                ftRenderLightmap.progressBarPercent = ftBuildGraphics.progressBarPercent;
                if (ftBuildGraphics.userCanceled)
                {
                    ftRenderLightmap.ProgressBarEnd();
                    yield break;
                }
                yield return null;
            }
        }
        else
        {
            if (!ftRenderLightmap.ValidateCurrentScene())
            {
                yield break;
            }
        }
        ftBuildGraphics.forceAllAreaLightsSelfshadow = false;
        if (exportLights)
        {
            ftBuildLights.InitMaps(true);
        }
        ftRenderLightmap.ProgressBarEnd();

        Active = this;
        GeneratePreviewSettings(false, false);

        if (renderInSceneView) ToggleSceneView(true);

        if (inputActive)
        {
            EndPreviewInput();
            inputActive = false;
        }
        pprocess = ftRenderLightmap.RunFTrace("interactive " + ftRenderLightmap.scenePathQuoted +  " a 4 0 0 preview.bin", true);
        if (pprocess == (System.IntPtr)0)
        {
            Debug.LogError("Failed to launch preview");
        }
        else
        {
            if (!StartPreviewInput())
            {
                Debug.LogError("Failed connecting to preview");
            }
            else
            {
                inputActive = true;
                EditorApplication.update += UpdateInput;
                EditorSceneManager.sceneOpened += OnSceneOpen;
            }
        }
        Repaint();
    }

    void StartPreview()
    {
        if (!progressFunc.MoveNext())
        {
            EditorApplication.update -= StartPreview;
        }
    }

    static void UpdateInput()
    {
        if (pprocess != (System.IntPtr)0)
        {
            if (!ftRenderLightmap.IsProcessFinished(pprocess))
            {
                int refreshTransforms = 0;
                for(int i=0; i<allDirects.Count; i++)
                {
                    if (allDirects[i] == null)
                    {
                        refreshTransforms = 2;
                        continue;
                    }
                    if (allDirects[i].transform.hasChanged)
                    {
                        refreshTransforms = 1;
                        allDirects[i].transform.hasChanged = false;
                    }
                }
                for(int i=0; i<allPoints.Count; i++)
                {
                    if (allPoints[i] == null)
                    {
                        refreshTransforms = 2;
                        continue;
                    }
                    if (allPoints[i].transform.hasChanged)
                    {
                        refreshTransforms = 1;
                        allPoints[i].transform.hasChanged = false;
                    }
                }
                for(int i=0; i<allSkies.Count; i++)
                {
                    if (allSkies[i] == null)
                    {
                        refreshTransforms = 2;
                        continue;
                    }
                    if (allSkies[i].transform.hasChanged)
                    {
                        refreshTransforms = 2; // needs full refresh due to cubemap being pre-tranformed
                        allSkies[i].transform.hasChanged = false;
                    }
                }

                if (lastIndirectBoost != ftRenderLightmap.hackIndirectBoost) refresh = 1;
                if (lastBackFaceWeight != ftRenderLightmap.giBackFaceWeight) refresh = 1;
                //if (lastEmissiveBoost != ftRenderLightmap.hackEmissiveBoost) refresh = 1;
                if (lastBounces != ftRenderLightmap.bounces) refresh = 1;

                if (renderInSceneView)
                {
                    var sceneViewRect = sceneView.camera.pixelRect;
                    if (pwidth != (int)sceneViewRect.width || pheight != (int)sceneViewRect.height)
                    {
                        InitPreviewImage();
                        refresh = 1;
                        waitForFrame = frameVer + 1;
                    }
                }

                if (BakeryPointLight.lightsChanged > 0 || BakeryDirectLight.lightsChanged > 0 || BakerySkyLight.lightsChanged > 0 ||
                    BakeryLightMesh.lightsChanged > 0 || refreshTransforms > 0 || refresh > 0)
                {
                    //Debug.LogError(refresh+" "+BakeryPointLight.lightsChanged+" "+ BakeryLightMesh.lightsChanged);
                    refresh = System.Math.Max(refresh, BakeryPointLight.lightsChanged);
                    refresh = System.Math.Max(refresh, BakeryDirectLight.lightsChanged);
                    refresh = System.Math.Max(refresh, BakerySkyLight.lightsChanged);
                    refresh = System.Math.Max(refresh, BakeryLightMesh.lightsChanged);
                    refresh = System.Math.Max(refresh, refreshTransforms);
                    BakeryPointLight.lightsChanged = 0;
                    BakeryDirectLight.lightsChanged = 0;
                    BakerySkyLight.lightsChanged = 0;
                    BakeryLightMesh.lightsChanged = 0;
                    GeneratePreviewSettings(refresh == 1, true);
                }

                if (inputActive)
                {
                    var cam = SceneView.lastActiveSceneView.camera;//Camera.current;
                    if (cam != null)
                    {
                        var tform = cam.transform;
                        camRht = tform.right;
                        camUp = tform.up;
                        camFwd = tform.forward;
                        camPos = tform.position;
                        camFov = Mathf.Tan(Mathf.Deg2Rad * cam.fieldOfView * 0.5f);

                        if (renderInSceneView && previewImageBuffer != null)
                        {
                            var uproj = cam.projectionMatrix;
                            var proj = GL.GetGPUProjectionMatrix(uproj, true);
                            var view = cam.worldToCameraMatrix;
                            camViewProj = proj * view;
                        }
                    }
                    if (exiting) camFov = -1;

                    if (refresh == 1)
                    {
                        refreshConstVer++;
                        //Debug.Log("Light refresh");
                    }
                    else if (refresh == 2)
                    {
                        refreshFullVer++;
                        //Debug.LogError("Full refresh");
                    }
                    refresh = 0;
                    if (!SendPreviewInput(camRht.x, camRht.y, camRht.z,
                                            camUp.x, camUp.y, camUp.z,
                                            camFwd.x, camFwd.y, camFwd.z,
                                            camPos.x, camPos.y, camPos.z,
                                            camFov, exposure, ftRenderLightmap.giBackFaceWeight, 1.0f,//ftRenderLightmap.hackEmissiveBoost,
                                            refreshConstVer, refreshFullVer, frameVer, psteps))
                    {
                        Debug.LogError("Failed to send input to preview");
                        StopPreview();
                    }
                    if (renderInSceneView && previewImageBuffer != null)
                    {
                        viewProjHistory[historyHead] = camViewProj;
                        frameVer++;
                        historyHead++;
                        if (historyHead == PREVIEW_HISTORY) historyHead = 0;

                        if (!GetPreviewImage(previewImageBuffer, previewImageBuffer.Length))
                        {
                            Debug.LogError("Failed to get preview image");
                            StopPreview();
                        }
                        int receivedFrame = System.BitConverter.ToInt32(previewImageBuffer, pwidth * pheight * 4);
                        if (waitForFrame == 0 || receivedFrame >= waitForFrame)
                        {
                            if (previewImageTex == null)
                            {
                                StopPreview();
                                return;
                            }
                            previewImageTex.LoadRawTextureData(previewImageBuffer);
                            previewImageTex.Apply();
                            //Debug.Log("Sent: "+frameVer+", Received: " + receivedFrame);
                            int historyIndex = historyHead;
                            if (frameVer - receivedFrame < PREVIEW_HISTORY)
                            {
                                int frameIndex = frameVer;
                                while(frameIndex > receivedFrame)
                                {
                                    frameIndex--;
                                    historyIndex--;
                                    if (historyIndex < 0) historyIndex = PREVIEW_HISTORY-1;
                                }
                            }
                            Shader.SetGlobalMatrix("_ftPreviewViewProj", viewProjHistory[historyIndex]);
                            waitForFrame = 0;
                        }
                    }
                }
            }
            else
            {
                pprocess = (System.IntPtr)0;
                StopPreview();
            }
        }
    }

	[MenuItem ("Bakery/Preview...", false, 1000)]
	public static void Preview ()
    {
        GetWindow(typeof(ftRenderLightmap));

        var instance = (ftPreview)GetWindow(typeof(ftPreview));
        instance.titleContent.text = "BakeryPreview";
        var edPath = ftLightmaps.GetEditorPath();
        var icon = EditorGUIUtility.Load(edPath + "icon.png") as Texture2D;
        instance.titleContent.image = icon;
        instance.Show();
	}
}

#endif
