using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Artanim;

public class TimelineControl : MonoBehaviour
{

    public PlayableController controller;

    public PlayableDirector timeline;
    public double speedIncrement = .05f;
    private bool gameInstance;
    private int currentOptionNum;
    private double startSpeed;
    private double currentSpeed;
    private bool skipButtonLock;
    private bool speedButtonLock;
    public double maxSpeedUp;
    

    [Header("ClientTriggers")]
    public ClientToServerTrigger PlayPauseTrigger;
    public ClientToServerTrigger FastForwardTrigger;
    public ClientToServerTrigger RewrindTrigger;
    public ClientToServerTrigger RestartTrigger;
    public ClientToServerTrigger SkipForwardTrigger;
    public ClientToServerTrigger SkipReverseTrigger;

    // Use this for initialization
    void Start()
    {
        if (GameController.Instance)
            gameInstance = true;
        controller = GetComponent<PlayableController>();
        timeline = GetComponent<PlayableDirector>();
    }


    //public void SkipAnimForward()
    //{
    //    StopAndTurnOffAll();
    //    int optionNum = currentOptionNum;
    //    optionNum++;
    //    if (optionNum > previsOptions.Length - 1)
    //        optionNum = 0;
    //    StartPrevis(optionNum);
    //}

    //public void SkipAnimReverse()
    //{
    //    StopAndTurnOffAll();
    //    int optionNum = currentOptionNum;
    //    optionNum--;
    //    if (optionNum < 0)
    //        optionNum = previsOptions.Length - 1;
    //    StartPrevis(optionNum);
    //}

    public void DisableTimelineControl()
    {
        this.enabled = false;
    }

    public void PlayPause()
    {
        if (timeline.playableGraph.GetRootPlayable(0).GetSpeed() == 1)
            controller.Pause();
        else if (timeline.playableGraph.GetRootPlayable(0).GetSpeed() == 0)
        {
            controller.Play();
        }
    }

    private void Update()
    {


        //float skipControl = Input.GetAxis("Skip Anim");
        float speedControl = Input.GetAxis("Speed Control");

        if (Input.GetKeyDown(KeyCode.Space))
        {

            PlayPause();
            //if (gameInstance && DevelopmentMode.CurrentMode != EDevelopmentMode.Standalone)
            //    PlayPauseTrigger.Trigger();
            //else
            //    PlayPause();
        }
        //else if (Input.GetButtonDown("Fire2"))
        //{
        //    if (gameInstance)
        //        RestartTrigger.Trigger();
        //    else
        //        RestartCurrent();
        //}

        //if (skipControl == 0)
        //{
        //    skipButtonLock = false;
        //}
        //else if (skipControl > 0 && !skipButtonLock)
        //{
        //    Debug.Log("Skip Anim: " + Input.GetAxis("Skip Anim"));
        //    skipButtonLock = true;
        //    if (gameInstance)
        //        SkipForwardTrigger.Trigger();
        //    else
        //        SkipAnimForward();
        //}
        //else if (skipControl < 0 && !skipButtonLock)
        //{
        //    Debug.Log("Skip Anim: " + Input.GetAxis("Skip Anim"));
        //    skipButtonLock = true;
        //    if (gameInstance)
        //        SkipReverseTrigger.Trigger();
        //    else
        //        SkipAnimReverse();
        //}

        //if (speedControl == 0)
        //{
        //    speedButtonLock = false;
        //}
        //else if (speedControl > 0 && !speedButtonLock)
        //{
        //    Debug.Log("Speed Control: " + Input.GetAxis("Speed Control"));
        //    speedButtonLock = true;
        //    if (gameInstance)
        //        FastForwardTrigger.Trigger();
        //    else
        //        FastForward();
        //}
        //else if (speedControl < 0 && !speedButtonLock)
        //{
        //    Debug.Log("Speed Control: " + Input.GetAxis("Speed Control"));
        //    speedButtonLock = true;
        //    if (gameInstance)
        //        RewrindTrigger.Trigger();
        //    else
        //        Rewind();
        //}
    }

    public void FastForward()
    {
        if (currentSpeed < 0)
        {
            currentSpeed = 0;
            timeline.Pause();
        }
        else if (currentSpeed == 0)
        {
            currentSpeed = 1;
            timeline.Play();
            timeline.playableGraph.GetRootPlayable(0).SetSpeed(currentSpeed);
        }
        else
        {
            timeline.Play();
            currentSpeed += speedIncrement;
            if (currentSpeed > maxSpeedUp)
                currentSpeed = 1;
            timeline.playableGraph.GetRootPlayable(0).SetSpeed(currentSpeed);
        }
    }

    public void Rewind()
    {
        if (currentSpeed > 0)
        {
            currentSpeed = 0;
            timeline.Pause();
        }
        else if (currentSpeed == 0)
        {
            timeline.Play();
            currentSpeed = -.5;
            timeline.playableGraph.GetRootPlayable(0).SetSpeed(currentSpeed);
        }
        else
        {
            timeline.Play();
            currentSpeed -= speedIncrement;
            if (currentSpeed < -maxSpeedUp)
                currentSpeed = -.5;
            timeline.playableGraph.GetRootPlayable(0).SetSpeed(currentSpeed);
        }
    }

    public void ResetSpeed()
    {
        timeline.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }

}

