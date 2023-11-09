using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DreamscapeUtil
{
    [CustomEditor(typeof(HideObjects))]
    [CanEditMultipleObjects]
    public class HideObjectsInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            bool hidden = false; ;

            for(int i = 0; i < targets.Length; i++)
            {
                HideObjects h = targets[i] as HideObjects;
                if (h.IsHiddenSelf())
                    hidden = true;
            }

            if (!Application.isPlaying)
            {
                GUI.enabled = false;
            }

            if (hidden)
            {
                if (GUILayout.Button("Show"))
                {
                    foreach(Object target in targets)
                    {
                        (target as HideObjects).Show();
                    }
                }
            }
            else
            {
                if (GUILayout.Button("Hide"))
                {
                    foreach (Object t in targets)
                    {
                        (t as HideObjects).Hide();
                    }
                }
            }


            GUI.enabled = true;

            EditorGUILayout.Space();

            base.OnInspectorGUI();
        }
    }
}
