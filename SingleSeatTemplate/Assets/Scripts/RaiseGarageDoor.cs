using UnityEngine;

public class RaiseGarageDoor : MonoBehaviour
{
    public float moveSpeed = 1.0f; // Speed at which the object moves up
    public float maxHeight = 5.0f; // Maximum height for the object

    private bool isMovingUp = false;
    public GameObject obj;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            isMovingUp = true;
        }


        if (isMovingUp)
        {
            // Move the object up in the y-axis
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

            // Check if the object has reached the maximum height
            if (transform.position.y >= maxHeight)
            {
                StopMoving();
            }
        }
       
            
    }

    public void StartRaising()
    {
        isMovingUp = true;
        obj.SetActive(true);
    }

    public void StopMoving()
    {
        isMovingUp = false;
        transform.Translate(Vector3.up * 0);
    }
}
