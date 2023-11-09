using UnityEngine;

public class ActivateSnowmobile : MonoBehaviour
{
    public GameObject button;

    public void OnButtonClick()
    {
        button.SetActive(true);
    }
}
