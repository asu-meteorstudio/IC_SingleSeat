using UnityEngine;

public class GeoFenceAlphaToggle : MonoBehaviour
{
    public Animator geoFenceAnimator;

    public void GeoFenceAlphaOff()
    {
        if(geoFenceAnimator != null)
        {
            geoFenceAnimator.SetBool("isAlphaOn", false);
        }
    }

    public void GeoFenceAlphaOn()
    {
        if (geoFenceAnimator != null)
        {
            geoFenceAnimator.SetBool("isAlphaOn", true);
        }
    }
}
