using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class AnimationAudioBehaviour : MonoBehaviour
{
    public List<AnimationAudioEventTable> AudioTable = new List<AnimationAudioEventTable>();

    public void PlayAudio(string evt)
    {
        Debug.LogFormat("PLAYING {0}", evt);

        var audios = AudioTable.FirstOrDefault(a => a.EventName.Equals(evt));
        if (audios == null)
            return;

        foreach(var audio in audios.AudioSources)
        {
            Debug.LogFormat("PLAYING {0} {1}", evt, audio.name);
            audio.Play();
        }
    }

    public void StopAudio(string evt)
    {
        Debug.LogFormat("STOPPING {0}", evt);

        var audios = AudioTable.FirstOrDefault(a => a.EventName.Equals(evt));
        if (audios == null)
            return;

        foreach (var audio in audios.AudioSources)
        {
            Debug.LogFormat("STOPPING {0} {1}", evt, audio.name);
            audio.Stop();
        }
    }
}

[Serializable]
public class AnimationAudioEventTable
{
    public string EventName;
    public List<AudioSource> AudioSources;
}
