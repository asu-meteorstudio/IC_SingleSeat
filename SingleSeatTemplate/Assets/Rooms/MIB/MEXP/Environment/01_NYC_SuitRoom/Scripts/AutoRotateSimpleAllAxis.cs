using UnityEngine;

public class AutoRotateSimpleAllAxis: MonoBehaviour
{
    [SerializeField] private float rotateDegreesPerSecondY;
	[SerializeField] private float rotateDegreesPerSecondZ;
	[SerializeField] private float rotateDegreesPerSecondX;

	void Update ()
	{
		transform.Rotate(rotateDegreesPerSecondX * Time.deltaTime, rotateDegreesPerSecondY * Time.deltaTime, rotateDegreesPerSecondZ * Time.deltaTime);
	}
}
