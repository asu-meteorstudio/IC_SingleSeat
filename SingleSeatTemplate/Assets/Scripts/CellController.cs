using Artanim;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    
    public Transform location1, location2;
    public Transform platform;
    public bool moving;

    [Header("Moving Variables)")]
    public float position = 0;
    public float speed = 0.1f;
    public AnimationCurve curve;

    // Start is called before the first frame update
    void Start()
    {
        moving = false;
        platform.position = location1.position;

        //StartCoroutine(GO());
    }

    private IEnumerator GO()
    {
        yield return new WaitForSeconds(5.5f);

        moving = true;
    }

    public void start()
    {
        moving = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (moving)
        {
            position += speed * Time.fixedDeltaTime;

            if (position >= 1)
            {
                position = 1;
                moving = false;
            }

            platform.position = Vector3.Lerp(location1.position, location2.position, curve.Evaluate(position));
        }
    }

    public void ToggleMoving()
    {
        moving = !moving;
    }

    private float TrapazoidFunction(float t)
    {
        if (position == 0)
        {
            return speed / 100f;
        }
        else if (position < 0.25f)
        {
            return speed * (position / 0.25f);
        }
        else if (position > 0.75f)
        {
            return speed * ((1.0f - position) / 0.25f);
        }
        else
        {
            return speed;
        }
    }
}
