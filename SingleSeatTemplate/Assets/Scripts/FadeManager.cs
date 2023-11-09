using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public Image fadeImage; // Reference to the UI Image used for fading
    public float fadeDuration = 1.5f; // Duration of the fade effect in seconds

    private bool backToBase = false; // Boolean to track whether to fade to black or not

    void Start()
    {
        // Ensure the fade image is visible at the start of the game
        fadeImage.color = Color.black;

        // Start the fade in effect
        FadeIn();
    }

    void Update()
    {
        // Check if backToBase is true, and initiate fade out if needed
        if (backToBase)
        {
            FadeOut();
        }
    }

    // Function to initiate the fade in effect
    void FadeIn()
    {
        fadeImage.CrossFadeAlpha(0f, fadeDuration, false);
    }

    // Function to initiate the fade out effect
    void FadeOut()
    {
        fadeImage.CrossFadeAlpha(1f, fadeDuration, false);
    }

    // Function to be called when the trigger box collision occurs
    public void TriggerFadeOut()
    {
        backToBase = true;
    }
}
