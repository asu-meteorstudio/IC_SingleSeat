using UnityEngine;
using UnityEngine.SceneManagement;

public class TetrahedralizeLP : MonoBehaviour
{
    void Start()
    {
        // Force Unity to synchronously regenerate the tetrahedral tesselation for all loaded Scenes
        LightProbes.Tetrahedralize();
    }
}