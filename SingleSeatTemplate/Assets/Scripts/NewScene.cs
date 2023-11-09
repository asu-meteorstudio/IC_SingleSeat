using UnityEngine;
using UnityEngine.SceneManagement;

public class NewScene : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.LoadScene("Scene 2", LoadSceneMode.Single);
    }
}