#if SCENE_CHECKER_BAKERY_EXTENSIONS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DreamscapeUtil
{
    public class BakeryLightChecks : ISceneCheckerGlobalSceneCheck
    {
        public int CheckForErrors(SceneCheckerContext context)
        {
            int numErrors = 0;

            SceneCheckerIterator it = SceneCheckerIterator.IterContext(context);

            while (it.MoveNext())
            {
                GameObject go = it.Current;

                if (go.TryGetComponent<Light>(out Light l))
                {

                    if (l.type == LightType.Directional)
                    {
                        BakeryDirectLight bdl = go.GetComponent<BakeryDirectLight>();
                        if (l.lightmapBakeType == LightmapBakeType.Realtime)
                        {
                            if (bdl)
                            {
                                Debug.LogWarningFormat(l, "Directional Light '{0}' is set to realtime, but it has a BakeryDirectLight component on it", l.name);
                                numErrors++;
                            }
                        }
                        else
                        {
                            if (!bdl)
                            {
                                Debug.LogWarningFormat(l, "Directional Light '{0}' does not have a BakeryDirectLight component on it", l.name);
                                numErrors++;
                            }
                            else
                            {
                                if (l.lightmapBakeType == LightmapBakeType.Baked)
                                {
                                    if (!bdl.bakeToIndirect)
                                    {
                                        Debug.LogWarningFormat(l, "Directional Light '{0}' is set to baked, but the BakeryDirectLight component is not set to bake direct and indirect", l.name);
                                        numErrors++;
                                    }

                                }
                                else if (l.lightmapBakeType == LightmapBakeType.Mixed)
                                {
                                    if (bdl.bakeToIndirect)
                                    {
                                        Debug.LogWarningFormat(l, "Directional Light '{0}' is set to mixed, but the BakeryDirectLight component is set to bake direct and indirect", l.name);
                                        numErrors++;
                                    }
                                }

                                if (bdl.color != l.color || bdl.intensity != l.intensity || bdl.indirectIntensity != l.bounceIntensity)
                                {
                                    Debug.LogWarningFormat(l, "Bakery settings do not match unity light settings - {0}", l.name);
                                    numErrors++;
                                }

                                if (l.shadows == LightShadows.None && bdl.samples > 0)
                                {
                                    Debug.LogWarningFormat(l, "Directional Light '{0}' is set to no shadows, but the BakeryDirectLight component does not have Shadow Samples set to 0", l.name);
                                    numErrors++;
                                }
                                else if (l.shadows != LightShadows.None && bdl.samples == 0)
                                {
                                    Debug.LogWarningFormat(l, "Directional Light '{0}' is set to cast shadows, but the BakeryDirectLight component has Shadow Samples set to 0", l.name);
                                    numErrors++;
                                }

                            }
                        }
                    }
                    else if (l.type == LightType.Point)
                    {
                        BakeryPointLight bpl = go.GetComponent<BakeryPointLight>();
                        if (l.lightmapBakeType == LightmapBakeType.Realtime)
                        {
                            if (bpl)
                            {
                                Debug.LogWarningFormat(l, "Point Light '{0}' is set to realtime, but it has a BakeryPointLight component on it", l.name);
                                numErrors++;
                            }
                        }
                        else
                        {
                            if (!bpl)
                            {
                                Debug.LogWarningFormat(l, "Point Light '{0}' does not have a BakeryPointLight component on it", l.name);
                                numErrors++;
                            }
                            else
                            {
                                if (l.lightmapBakeType == LightmapBakeType.Baked)
                                {
                                    if (!bpl.bakeToIndirect)
                                    {
                                        Debug.LogWarningFormat(l, "Point Light '{0}' is set to baked but the BakeryPointLight is not set to bake direct and indirect", l.name);
                                        numErrors++;
                                    }
                                }
                                else if (l.lightmapBakeType == LightmapBakeType.Mixed)
                                {
                                    if (bpl.bakeToIndirect)
                                    {
                                        Debug.LogWarningFormat(l, "Point Light '{0}' is set to mixed but the BakeryPointLight is set to bake direct and indirect", l.name);
                                        numErrors++;
                                    }
                                }

                                if (bpl.color != l.color || bpl.intensity != l.intensity || bpl.indirectIntensity != l.bounceIntensity ||
                                    bpl.projMode != BakeryPointLight.ftLightProjectionMode.Omni || bpl.cutoff != l.range)
                                {
                                    Debug.LogWarningFormat(l, "Bakery settings do not match unity light settings - {0}", l.name);
                                    numErrors++;
                                }

                                if (l.shadows == LightShadows.None && bpl.samples > 0)
                                {
                                    Debug.LogWarningFormat(l, "Point Light '{0}' is set to no shadows, but the BakeryPointLight component does not have Samples set to 0", l.name);
                                    numErrors++;
                                }
                                else if (l.shadows != LightShadows.None && bpl.samples == 0)
                                {
                                    Debug.LogWarningFormat(l, "Point Light '{0}' is set to cast shadows, but the BakeryPointLight component has Samples set to 0", l.name);
                                    numErrors++;
                                }

                            }
                        }
                    }
                    else if (l.type == LightType.Spot)
                    {
                        BakeryPointLight bpl = go.GetComponent<BakeryPointLight>();
                        if (l.lightmapBakeType == LightmapBakeType.Realtime)
                        {
                            if (bpl)
                            {
                                Debug.LogWarningFormat(l, "Spot Light '{0}' is set to realtime, but it has a BakeryPointLight component on it", l.name);
                                numErrors++;
                            }
                        }
                        else
                        {
                            if (!bpl)
                            {
                                Debug.LogWarningFormat(l, "Spot Light '{0}' does not have a BakeryPointLight component on it", l.name);
                                numErrors++;
                            }
                            else
                            {
                                if (l.lightmapBakeType == LightmapBakeType.Baked)
                                {
                                    if (!bpl.bakeToIndirect)
                                    {
                                        Debug.LogWarningFormat(l, "Spot Light '{0}' is set to baked but the BakeryPointLight is not set to bake direct and indirect", l.name);
                                        numErrors++;
                                    }
                                }
                                else if (l.lightmapBakeType == LightmapBakeType.Mixed)
                                {
                                    if (bpl.bakeToIndirect)
                                    {
                                        Debug.LogWarningFormat(l, "Spot Light '{0}' is set to mixed but the BakeryPointLight is set to bake direct and indirect", l.name);
                                        numErrors++;
                                    }
                                }

                                if (bpl.color != l.color || bpl.intensity != l.intensity || bpl.indirectIntensity != l.bounceIntensity ||
                                    bpl.projMode != BakeryPointLight.ftLightProjectionMode.Cookie || bpl.cutoff != l.range || bpl.cookie == null ||
                                    bpl.angle != l.spotAngle)
                                {
                                    Debug.LogWarningFormat(l, "Bakery settings do not match unity light settings - {0}", l.name);
                                    numErrors++;
                                }

                                if (l.shadows == LightShadows.None && bpl.samples > 0)
                                {
                                    Debug.LogWarningFormat(l, "Spot Light '{0}' is set to no shadows, but the BakeryPointLight component does not have Samples set to 0", l.name);
                                    numErrors++;
                                }
                                else if (l.shadows != LightShadows.None && bpl.samples == 0)
                                {
                                    Debug.LogWarningFormat(l, "Spot Light '{0}' is set to cast shadows, but the BakeryPointLight component has Samples set to 0", l.name);
                                    numErrors++;
                                }

                            }
                        }
                    }

                }
            }

            return numErrors;
        }
    }
}
#endif