using UnityEngine;
using System.Collections.Generic;

public class Follow2Targets : MonoBehaviour
{
    [SerializeField] GameObject TargetA;
    [SerializeField] private int m_targetAHitsBeforeTransition = 3;
    [SerializeField] GameObject TargetB;
    [SerializeField] private int m_targetBHitsBeforeTransition = 3;
    [SerializeField] private float speed = 1f;
    [SerializeField] List<GameObject> colliders;

    const string TRIGGERTAG = "Trigger";
    private Transform targetATransform;
    private Transform targetBTransform;
    private int targetAHitsStart;
    private int targetBHitsStart;

    private float weight = 0f;
    private bool transitionToTargetB;
    private bool isTransitioning = false;

    Dictionary<int, GameObject> m_triggers = new Dictionary<int, GameObject>();

    void Start ()
    {
        targetATransform = TargetA.transform;
        targetBTransform = TargetB.transform;
        targetAHitsStart = m_targetAHitsBeforeTransition;
        targetBHitsStart = m_targetBHitsBeforeTransition;

        m_triggers.Clear();
        List<GameObject> gos = new List<GameObject>();
        foreach (GameObject go in colliders)
        {
            if (gos.Contains(go)) continue;
            gos.Add(go);
            m_triggers[go.GetInstanceID()] = go;
        }
    }
	
	void LateUpdate ()
	{
	    transform.position = Vector3.Lerp(targetATransform.position, targetBTransform.position, EaseInEaseOut(weight, 0f, 1f, 1f));
	    transform.rotation = Quaternion.Lerp(targetATransform.rotation, targetBTransform.rotation, EaseInEaseOut(weight, 0f, 1f, 1f));

	    if (isTransitioning)
	    {
	        if (transitionToTargetB)
	        {
	            weight += Time.deltaTime * speed;
	            if (weight >= 1.0f)
	            {
	                weight = 1.0f;
	                isTransitioning = false;
	            }
	        }
	        if (!transitionToTargetB)
	        {
	            weight -= Time.deltaTime * speed;
	            if (weight <= 0f)
	            {
	                weight = 0f;
	                isTransitioning = false;
	            }
	        }
        }
	}

    private float EaseInEaseOut(float t, float b, float c, float d)
    {
        t /= d / 2;
        if (t < 1) return c / 2 * t * t + b;
        t--;
        return -c / 2 * (t * (t - 2) - 1) + b;
    }

    private void SwitchTarget()
    {
        transitionToTargetB = !transitionToTargetB;
        isTransitioning = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(TRIGGERTAG) && m_triggers.ContainsKey(other.gameObject.GetInstanceID()))
        {
            if (!transitionToTargetB && !isTransitioning)
            {
                targetAHitsStart--;
                if (targetAHitsStart == 0)
                {
                    SwitchTarget();
                    targetAHitsStart = m_targetAHitsBeforeTransition;
                }
            }

            if (transitionToTargetB && !isTransitioning)
            {
                targetBHitsStart--;
                if (targetBHitsStart == 0)
                {
                    SwitchTarget();
                    targetBHitsStart = m_targetBHitsBeforeTransition;
                }
            }
        }
    }
}
