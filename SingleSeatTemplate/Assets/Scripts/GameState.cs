using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Singleton; // one instance of this class
    public bool introState; // intro game state (BEFORE movement)
    public bool traverseState; // movement game state (when user is moving towards pump site)
    public bool pumpState; // pump game state (when user ARRIVES to pump state)


    // dialogue
    public bool penguinDialogue;

    // Start is called before the first frame update
    void Start()
    {
        Singleton = this;
        introState = true;
        traverseState = false;
        pumpState = false;

        penguinDialogue = false;
    }

    public void ToggleTraverseState()
    {
        introState = false;
        traverseState = true;
        pumpState = false;
    }

    public void TogglePumpState()
    {
        introState = false;
        traverseState = false;
        pumpState = true;
    }
}
