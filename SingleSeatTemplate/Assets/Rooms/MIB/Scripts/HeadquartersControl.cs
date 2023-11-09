using FluffyUnderware.Curvy.Controllers;
using System.Collections;
using UnityEngine;

public class HeadquartersControl : DreamscapeSceneControl
{
    #region Singleton Behaviour
    private static HeadquartersControl _instance;
    public static bool HasInstance
    {
        get
        {
            return _instance != null;
        }
    }
    public static HeadquartersControl Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<HeadquartersControl>();
            }
            return _instance;
        }

        protected set
        {
            _instance = value;
        }
    }
    #endregion

    private void Start()
    {
        _instance = this;
    }

    public void TurnOnButton()
    {
        HeadquartersSubstationControl.Instance.TurnOnButton();
    }

    public void PlayNurleneSubtitle()
    {
        Artanim.SubtitleController.Instance.ShowSubtitle("042_NurleneYoureLateHes", true, 6.5f, false, false, false);
    }
}
