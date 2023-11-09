using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetLocalAnimation : MonoBehaviour
{
    public bool resetLocalPosition = false;
    public bool resetLocalRotation = false;
    public bool resetLocalScale = false;

    Vector3 _localPosition;
    Quaternion _localRotation;
    Vector3 _localScale;
    
    void Start()
    {
        _localPosition = transform.localPosition;

        _localRotation = transform.localRotation;

        _localScale = transform.localScale;
    }

    void LateUpdate()
    {
        if (resetLocalPosition) transform.localPosition = _localPosition;

        if (resetLocalRotation) transform.localRotation = _localRotation;

        if (resetLocalScale) transform.localScale = _localScale;

    }
}
