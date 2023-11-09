using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VCButtonGenerator : MonoBehaviour
{
    public class VCScene {
        public string sceneName;
        public string label;
        public string photosphere;
        public VCScene(string name, string lab, string photo)
        {
            sceneName = name;
            label = lab;
            photosphere = photo;
        }
    }
    List<VCScene> sceneList;
    public GameObject PFB_Button;
    public float xspacing, yspacing;
       
    // Start is called before the first frame update
    void Start()
    {
        xspacing = 0.3f;
        yspacing = 0.3f;
        sceneList = new List<VCScene>();

        //Eventually put this in a JSON. Make sure the HDR photospheres are in the Resources folder.
        sceneList.Add(new VCScene("classroom","Classroom","HDR-Classroom"));
        sceneList.Add(new VCScene("ConferenceRoom", "Meeting Room", "HDR_MeetingRoom"));
        sceneList.Add(new VCScene("Apollo_MissionControl", "Apollo Command Center", "HDR_Apollo_MissionControl"));
        sceneList.Add(new VCScene("Moon_Env", "Moon", "HDR-Moon"));
        sceneList.Add(new VCScene("Underwater", "Underwater", "HDR-Underwater"));
        sceneList.Add(new VCScene("DarkCity", "Dark City", "HDR-DarkCity"));
        sceneList.Add(new VCScene("EgyptianTomb", "Egyptian Tomb", "HDR-EgyptianTomb"));
        sceneList.Add(new VCScene("Museum_Roman", "Museum", "HDR_RomeMuseum"));
        sceneList.Add(new VCScene("TheaterToJungle_Main", "Jungle", "HDR-Jungle"));
        sceneList.Add(new VCScene("GreekTheater", "Greek Theater", "HDR-GreekTheater"));
        sceneList.Add(new VCScene("AlienZoo_MainVista", "Alien Zoo", "HDR_AlienZoo_MainVista"));
        sceneList.Add(new VCScene("Ocean_Boat", "Ocean", "HDR-OceanBoat"));
        sceneList.Add(new VCScene("Gladiator_Arena", "Gladiator Arena", "HDR-Gladiator_Arena"));
        sceneList.Add(new VCScene("University35", "University", "HDR-University"));
        sceneList.Add(new VCScene("05_LON_Headquarters_moving", "MIB", "HDR-MIB"));
        sceneList.Add(new VCScene("HagiaSophia", "Hagia Sophia", "HDR-HagiaSophia"));
        sceneList.Add(new VCScene("ExternalCell", "External Cell", "HDR-ExternalCell"));
        sceneList.Add(new VCScene("BarrioCepo", "Barrio", "HDR-Barrio"));
        sceneList.Add(new VCScene("Church_interior", "Church", "HDR-Church"));
        sceneList.Add(new VCScene("TileMakerVC", "JMARS", "HDR-Tile"));

        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Generate()
    {
        int i=0;
        int rowlength = sceneList.Count / 3;
        foreach (VCScene scene in sceneList) {
            GameObject obj = Instantiate(PFB_Button, transform);
            obj.name = "Button "+scene.sceneName;
            Artanim.ProfessorTrigger button = obj.GetComponent<Artanim.ProfessorTrigger>();
            
            //Place button
            obj.transform.localPosition = new Vector3(0, (i / rowlength) * yspacing, -i % rowlength * xspacing);

            //Link scenename button
            button.ObjectId = scene.sceneName;

            button.OnHandEnter.RemoveAllListeners();
            button.OnHandEnter.AddListener((a) => obj.GetComponent<SceneControllButton>().LoadScene(scene.sceneName));

            //Change photosphere
            ReflectionProbe probe = obj.GetComponentInChildren<ReflectionProbe>();
            probe.customBakedTexture = Resources.Load(scene.photosphere) as Texture;
            //Label button
            UnityEngine.UI.Text label = obj.GetComponentInChildren<UnityEngine.UI.Text>();
            label.text = scene.label;

            i++;
        }
        
    }
}
