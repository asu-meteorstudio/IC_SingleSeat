using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateScene2 : MonoBehaviour
{
    public static GameStateScene2 Singleton; // one instance of this class
    public bool introState; // intro game state (BEFORE movement)
    public bool traverseState; // movement game state (when user moves to screen)
    public bool AISimulationState; // AI simulation game state (when user ARRIVES to pump state)


    // dialogue
    public bool AIDialogue;

    // Start is called before the first frame update
    void Start()
    {
        Singleton = this;
        introState = true;
        traverseState = false;
        AISimulationState = false;

        AIDialogue = false;
    }

    public void ToggleTraverseState()
    {
        introState = false;
        traverseState = true;
        AISimulationState = false;
    }

    public void TogglePumpState()
    {
        introState = false;
        traverseState = false;
        AISimulationState = true;
    }
}
