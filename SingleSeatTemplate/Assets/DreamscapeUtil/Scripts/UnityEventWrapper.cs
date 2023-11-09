using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DreamscapeUtil
{
    /// <summary>
    /// Simple wrapper around UnityEvent. Allows for easy grouping of function calls
    /// </summary>
    public class UnityEventWrapper : MonoBehaviour
    {
        public UnityEvent actions;

        public void Invoke()
        {
            actions.Invoke();
        }
    }
}
