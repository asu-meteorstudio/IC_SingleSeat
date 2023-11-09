using UnityEngine;

public class AutoRotateSimple : MonoBehaviour
{
    [SerializeField] private float rotateDegreesPerSecond;

	void Update ()
	{
        transform.Rotate(0, rotateDegreesPerSecond * Time.deltaTime, 0);
	}
}
