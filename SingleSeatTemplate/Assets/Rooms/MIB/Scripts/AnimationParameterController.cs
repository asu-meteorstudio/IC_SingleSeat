using UnityEngine;

public class AnimationParameterController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private int animationToPlay = 0;
    [SerializeField] private bool overrideRandomParameter = true;
    [SerializeField] private bool drawGizmo = true;

    //private AnimationRandomParameterController animRandParamController;
    private const string TRIGGER = "Trigger";
    private int paramID;
    private int lastAnimPlayedFromRPC;
    
    void Start ()
	{
	    paramID = Animator.StringToHash("action");
	    //animRandParamController = animator.gameObject.GetComponent<AnimationRandomParameterController>();
        //lastAnimPlayedFromRPC = animRandParamController.AnimationToPlay;
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TRIGGER))
        {
            animator.SetInteger(paramID, animationToPlay);
            //animRandParamController.SetTriggerOverride = overrideRandomParameter;
            //if (overrideRandomParameter)
            //{
            //    animRandParamController.AnimationToPlay = animationToPlay;
            //}
            //else animRandParamController.AnimationToPlay = lastAnimPlayedFromRPC;
        }
    }
    
    void OnDrawGizmos()
    {
        if (drawGizmo)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));
        }
    }

    void OnDrawGizmosSelected()
    {
        if (drawGizmo)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));
        }
    }
}
