using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SRL
{
    public class DoAfterDelay : MonoBehaviour
    {
        [SerializeField] private float delay;

        [SerializeField] private UnityEvent thingToDo;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(delay);
            
            thingToDo.Invoke();
        }
    }
}