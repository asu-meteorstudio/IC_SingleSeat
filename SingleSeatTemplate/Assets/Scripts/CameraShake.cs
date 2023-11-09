using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Camera Information
    public Transform cameraTransform;
    private Vector3 orignalCameraPos;

    // Shake Parameters
    public float shakeDuration = 2f;
    public float shakeAmount = 0.1f;

    private bool canShake = false;
    private float _shakeTimer;



    // Start is called before the first frame update
    void Start()
    {
        orignalCameraPos = cameraTransform.localPosition; // gets original camera position
    }

    // Update is called once per frame
    void Update()
    {
       
        if (canShake)
        {
            StartCameraShakeEffect();
        }
        
    }

    public void ShakeCamera()
    {
        canShake = true;
        _shakeTimer = shakeDuration;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SnowMobile"))
        {
            print("true");
            ShakeCamera();
        }
    }

    public void StartCameraShakeEffect()
    {
        if (_shakeTimer > 0)
        {
            // shake camera
            cameraTransform.localPosition = orignalCameraPos + Random.insideUnitSphere * shakeAmount;
            _shakeTimer -= Time.deltaTime; // decrease timer
        }
        else
        {
            // reset camera to starting position when done shaking
            _shakeTimer = 0f;
            cameraTransform.position = orignalCameraPos;
            canShake = false;
        }
    }

}
