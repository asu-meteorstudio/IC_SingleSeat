using DreamscapeUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEventHelper : MonoBehaviour, ISceneCheckerBehaviour
{
    [System.Serializable]
    public struct NamedAudioClip
    {
        public string name;
        public AudioClip clip;
    }

    public NamedAudioClip[] clips;
    [NonNull]
    public AudioSource source;

    public void PlayClip(string clipName)
    {
        if (Application.isPlaying)
        {
            foreach (NamedAudioClip clip in clips)
            {
                if (clip.name == clipName)
                {
                    source.PlayOneShot(clip.clip);
                }
            }
        }
    }

    public int CheckForErrors()
    {
        int numErrors = 0;
        for(int i = 0; i < clips.Length; i++)
        {
            if (string.IsNullOrEmpty(clips[i].name))
            {
                Debug.LogWarningFormat("Clip Name cannot be empty - {0}", this.name);
                numErrors++;
            }
            if (!clips[i].clip)
            {
                Debug.LogWarningFormat("Audio clip cannot be null - {0}", this.name);
                numErrors++;
            }
            for(int j = 0; j < i; j++)
            {
                if(!string.IsNullOrEmpty(clips[i].name) && clips[i].name == clips[j].name)
                {
                    Debug.LogWarningFormat("Duplicate clip name: '{0}' - {1}", clips[i].name, this.name);
                    numErrors++;
                }
            }
        }

        return numErrors;
    }
}
