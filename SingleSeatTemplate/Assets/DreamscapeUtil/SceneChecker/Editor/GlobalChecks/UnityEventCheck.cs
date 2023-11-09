using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
#if UNITY_2018_1_OR_NEWER
using UnityEngine.Timeline;
#endif

namespace DreamscapeUtil
{
    [Description("Checks for missing references/functions in UnityEvents and signal receivers")]
    public class UnityEventCheck : ISceneCheckerGlobalSceneCheck
    {
        public int CheckForErrors(SceneCheckerContext context)
        {
            int numErrors = 0;

            SceneCheckerIterator it = SceneCheckerIterator.IterContext(context);

            while (it.MoveNext())
            {
                foreach (Behaviour behaviour in it.Current.GetComponents<Behaviour>())
                {
                    if (!behaviour)
                    {
                        continue;
                    }
                    Type t = behaviour.GetType();
                    foreach (FieldInfo fieldInfo in t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (typeof(UnityEventBase).IsAssignableFrom(fieldInfo.FieldType))
                        {
                            UnityEventBase evt = (UnityEventBase)fieldInfo.GetValue(behaviour);
                            numErrors += CheckUnityEvent(evt, behaviour, fieldInfo.Name);
                        }
                    }
                }


#if UNITY_2018_1_OR_NEWER
                //check signal receivers
                SignalReceiver receiver = it.Current.GetComponent<SignalReceiver>();
                if (receiver)
                {
                    for(int i = 0; i < receiver.Count(); i++)
                    {
                        SignalAsset signalAsset = receiver.GetSignalAssetAtIndex(i);
                        if(signalAsset == null)
                        {
                            numErrors++;
                            Debug.LogWarningFormat(receiver, "SignalReceiver does not specify a signal asset - {0}", receiver.name);
                        }
                        UnityEvent evt = receiver.GetReactionAtIndex(i);

                        string s = string.Format("Reaction to signal '{0}'", signalAsset);
                        numErrors += CheckUnityEvent(evt, receiver, s);
                    }
                }

#endif
            }

            return numErrors;

        }


        //checks unityevent for null references/missing functions
        protected static int CheckUnityEvent(UnityEventBase evt, Behaviour context, string fieldName)
        {
            int numErrors = 0;

            Type t = context.GetType();

            for (int i = 0; i < evt.GetPersistentEventCount(); i++)
            {
                UnityEngine.Object target = evt.GetPersistentTarget(i);
                string methodName = evt.GetPersistentMethodName(i);

                if (!target)
                {
                    Debug.LogWarningFormat(context, "Null reference in UnityEvent '{0}' - {1}({2})", fieldName, t.Name, context.name);
                    numErrors++;
                }
                else if (string.IsNullOrEmpty(methodName))
                {
                    Debug.LogWarningFormat(context, "Missing method name in UnityEvent {0} - {1}({2})", fieldName, t.Name, context.name);
                    numErrors++;
                }
                else
                {
                    if (!HasPublicMethod(target.GetType(), methodName))
                    {
                        Debug.LogWarningFormat(context, "UnityEvent {0} references a method that no longer exists or is not public({3}) - {1}({2})",
                            fieldName, t.Name, context.name, methodName);
                        numErrors++;
                    }
                }
            }

            return numErrors;
        }

        protected static bool HasPublicMethod(Type t, string methodName)
        {

            foreach (MethodInfo info in t.GetMethods())
            {
                if (info.Name == methodName && info.IsPublic)
                {
                    return true;
                }
            }

            return false;
        }

    }
    
}
