using UnityEngine;
using UnityEngine.Playables;
using System.Collections;
using UnityEngine.Animations;
using UnityEngine.Timeline;

//[ExecuteInEditMode]
public class PlayableController : MonoBehaviour
{
    public PlayableDirector PlayableDirector;

    public string Id;
    //public string ParentId;

    TimelineManager timelineManager {  get { return TimelineManager.Instance; } }

    public bool ShouldContinue;

    void Reset()
    {
        if (!PlayableDirector)
        {
            PlayableDirector = GetComponent<PlayableDirector>();
        }
    }

    IEnumerator Start()
    {
        while (!timelineManager)
            yield return null;

        if(string.IsNullOrEmpty(Id))
        {
            Debug.LogWarning("Set an ID for the PlayableController on " + gameObject.name);
        }
        else
        {
            timelineManager.Register(this);
        }
    }

    void OnDestroy()
    {
        if(timelineManager)
            timelineManager.Deregister(this);
    }

    [ContextMenu("Play")]
    public void Play()
    {
        //Debug.Log("Attempting Play on " + this.name);
        timelineManager.Play(this);
    }

    [ContextMenu("Pause")]
    public void Pause()
    {
        //Debug.Log("Attempting Pause on " + this.name);
        timelineManager.Pause(this);
    }

    public void Pause(TimelineClip clip)
    {
        //Debug.Log("Attempting Pause on " + this.name + " at " + clip.start);
        timelineManager.Pause(this, clip);
    }

    //[ContextMenu("Go To Time")]
    public void GoToTime(float time)
    {
        //Debug.Log("Attempting GoToTime " + time + " " + this.name);
        timelineManager.GoToTime(this, time);
    }

    [ContextMenu("Continue")]
    public void Continue()
    {
        //Debug.Log("Attempting Continue on " + this.name);
        timelineManager.Continue(this);
    }
}

//[System.Serializable]
//public class AnimatorToFloatTable : SerializableDictionary<Animator, float> { }