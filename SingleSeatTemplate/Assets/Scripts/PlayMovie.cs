using UnityEngine;
using UnityEngine.Video;

public class PlayMovie : MonoBehaviour
{
    public bool playVideo = false;
    public bool pauseVideo = false;
    public bool playSlide1 = false;
    public bool playSlide2 = false;
    public bool playSlide3 = false;
    public bool playSlide4 = false;

    public Material blankScreenMaterial;
    public Material slide1ScreenMaterial;
    public Material slide2ScreenMaterial;
    public Material slide3ScreenMaterial;
    public Material slide4ScreenMaterial;

    private bool previousToggle = false;
    private MeshRenderer meshRenderer;

    private VideoPlayer videoPlayer;

    private void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (playVideo)
        {
            Play();
            playVideo = false;
        }

        if (pauseVideo)
        {
            Pause();
            pauseVideo = false;
        }

        if (playSlide1)
        {
            PlaySlide1();
            playSlide1 = false;
        }

        if (playSlide2)
        {
            PlaySlide2();
            playSlide2 = false;
        }

        if (playSlide3)
        {
            PlaySlide3();
            playSlide3 = false;
        }

        if (playSlide4)
        {
            PlaySlide4();
            playSlide4 = false;
        }
    }

    public void Play()
    {
        videoPlayer.Play();
    }

    public void Pause()
    {
        videoPlayer.Pause();
    }

    public void PlaySlide1()
    {
        videoPlayer.Stop();
        meshRenderer.material = slide1ScreenMaterial;
    }

    public void PlaySlide2()
    {
        videoPlayer.Stop();
        meshRenderer.material = slide2ScreenMaterial;
    }

    public void PlaySlide3()
    {
        videoPlayer.Stop();
        meshRenderer.material = slide3ScreenMaterial;
    }

    public void PlaySlide4()
    {
        videoPlayer.Stop();
        meshRenderer.material = slide4ScreenMaterial;
    }
}