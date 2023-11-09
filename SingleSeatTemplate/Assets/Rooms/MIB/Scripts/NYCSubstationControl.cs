using UnityEngine;

public class NYCSubstationControl : DreamscapeSceneControl
{
    #region Singleton Behaviour
    private static NYCSubstationControl _instance;
    public static bool HasInstance
    {
        get
        {
            return _instance != null;
        }
    }
    public static NYCSubstationControl Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<NYCSubstationControl>();
            }
            return _instance;
        }

        protected set
        {
            _instance = value;
        }
    }
    #endregion

    public Animator endOfLineWallAnim;
    public Animator FrankAnimator;

    void Start ()
    {
        _instance = this;
	}

    public void OpenEndOfLineWall()
    {
        endOfLineWallAnim.SetTrigger("Open");
    }

    public void FrankFinishedNY()
    {
        EventManager.Publish(new FrankFinishedNYEvent());
    }
}

public class FrankFinishedNYEvent { }
