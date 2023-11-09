using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToMarsFloor : MonoBehaviour
{
    public Transform globalMocapOffsetTransform;

    public Transform stationTransform;

    public Transform surfaceTransform;

    public List<GameObject> objectsToEnableOnSurface;

    public List<AudioSource> stationSounds;
    public List<AudioSource> surfaceSounds;

    public void GoToSurface()
    {
        globalMocapOffsetTransform.position = surfaceTransform.position;
        globalMocapOffsetTransform.rotation = surfaceTransform.rotation;

        foreach (GameObject go in objectsToEnableOnSurface)
            go.SetActive(true);

        foreach (var audioSource in stationSounds)
            audioSource.Stop();

        foreach (var audioSource in surfaceSounds)
            audioSource.Play();
    }

    public void GoToStation()
    {
        globalMocapOffsetTransform.position = stationTransform.position;
        globalMocapOffsetTransform.rotation = stationTransform.rotation;
        foreach (GameObject go in objectsToEnableOnSurface)
            go.SetActive(false);

        foreach (var audioSource in stationSounds)
            audioSource.Play();

        foreach (var audioSource in surfaceSounds)
            audioSource.Stop();
    }
}
