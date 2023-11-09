using Artanim.Location.Data;
using Artanim.Location.Network;
using Artanim.Location.SharedData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using Artanim;

public class TileController : MonoBehaviour
{
    [Header("Tile Variables")]
    private float PlanetScale = 50f, TableScale = 1f;
    public TextureLoader textureloaderDesk, textureloaderPlanet;
    public Transform TileMakerDesk, TileMakerPlanet;
    public GameObject PlanetGO;
    public bool Planet;
    private int currentpos;
    public GameObject[] buttons;

    [Header("Tilemaker Positions")]
    public Transform DeskLocation;
    public Transform[] PlanetLocations;


    // Start is called before the first frame update
    void Start()
    {
        textureloaderDesk.LoadIndex(0);
        currentpos = 0;
        Planet = false;
        PlanetGO.SetActive(false);

        if (NetworkInterface.Instance.IsClient && GameController.Instance.CurrentPlayer.Player.Avatar.Contains("prof"))
        {
            foreach (GameObject go in buttons)
            {
                go.GetComponent<MeshRenderer>().enabled = false;
            }
        }

        //SwitchToPlanet();
        //LoadIndex(3);
    }

    [Button]
    public void SwitchToPlanet()
    {
        Planet = !Planet;

        if (!Planet) //switch to planet location
        {
            PlanetGO.SetActive(true);
            LoadIndex(currentpos);
            //TileMakerDesk.position = PlanetLocations[currentpos].position;
            //textureloaderDesk.LoadIndex(currentpos, PlanetScale);
        }
        else
        {
            PlanetGO.SetActive(false);
            LoadIndex(currentpos);
            //TileMakerDesk.position = DeskLocation.position;
            //textureloaderDesk.LoadIndex(currentpos, TableScale);
        }        
    }

    public void LoadIndex(int i)
    {
        currentpos = i;        

        if (Planet)
        {
            textureloaderDesk.LoadIndex(currentpos, TableScale);
            textureloaderPlanet.LoadIndex(currentpos, PlanetScale);
            TileMakerPlanet.position = PlanetLocations[currentpos].position;
        }            
        else
            textureloaderDesk.LoadIndex(currentpos, TableScale);
    }

    [Button]
    public void Load1()
    {
        LoadIndex(0);
    }

    [Button]
    public void Load2()
    {
        LoadIndex(1);
    }

    [Button]
    public void Load3()
    {
        LoadIndex(2);
    }

    [Button]
    public void Load4()
    {
        LoadIndex(3);
    }

    [Button]
    public void Load5()
    {
        LoadIndex(4);
    }
}
