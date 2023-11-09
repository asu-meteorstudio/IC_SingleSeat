using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace DreamscapeUtil
{

    public interface ISceneCheckerAttribute
    {
        ///<summary>
        ///Returns true if field passes the test, false if not.
        ///Should log a useful warning message when returning false
        ///</summary>
        bool CheckField(UnityEngine.Object obj, FieldInfo fieldInfo);
    }

    ///     <summary>
    ///      Specifies that the value of a field should not be null.
    ///      For array values, no element of the array can be null
    ///     </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class NonNull : Attribute, ISceneCheckerAttribute
    {
        public bool CheckField(UnityEngine.Object obj, FieldInfo fieldInfo)
        {
            var value = fieldInfo.GetValue(obj);
            
            if (value is UnityEngine.Object)
            {
                UnityEngine.Object unityObj = (UnityEngine.Object)value;
                //for anything that inherits from UnityEngine.Object, the ! operator will return true if the object has been destroyed or th reference is otherwise invalid
                if (!unityObj)
                {
                    Debug.LogWarningFormat(obj, "{0} is null - {1}({2})", fieldInfo.Name, obj.GetType().Name, obj.name);
                    return false;
                }
            }

            else if(value == null){
                //need exception for arrays/lists - empty arrays sometimes get returned as null for some reason
                if (!typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
                {
                    Debug.LogWarningFormat(obj, "{0} is null - {1}({2})", fieldInfo.Name, obj.GetType().Name, obj.name);
                    return false;
                }          
            }
            //for lists/arrays - iterate through and check each element to see if it's null
            if (value is IList)
            {
                IEnumerable enumerableObj = (IEnumerable)value;

                foreach (var elem in enumerableObj)
                {
                    if (elem is UnityEngine.Object)
                    {
                        UnityEngine.Object unityObj = (UnityEngine.Object)elem;
                        if (!unityObj)
                        {
                            Debug.LogWarningFormat(obj, "{0} contains null elements - {1}({2})", fieldInfo.Name, obj.GetType().Name, obj.name);
                            return false;
                        }
                    }
                    else if (elem == null)
                    {
                        Debug.LogWarningFormat(obj, "{0} contains null elements - {1}({2})", fieldInfo.Name, obj.GetType().Name, obj.name);
                        return false;
                    }
                }
            }

            return true;
        }
    }
    /// <summary>
    /// Specifies that an array value must have at least one element.
    /// For string values, value cannot be the empty string
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class NonEmpty : Attribute, ISceneCheckerAttribute
    {
        public bool CheckField(UnityEngine.Object obj, FieldInfo fieldInfo)
        {
            var value = fieldInfo.GetValue(obj);
            if (value is IEnumerable)
            {
                IEnumerable enumerableObj = (IEnumerable)value;

                if (!enumerableObj.GetEnumerator().MoveNext())
                {
                    Debug.LogWarningFormat(obj, "{0} is empty - {1}({2})", fieldInfo.Name, obj.GetType().Name, obj.name);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                Debug.LogWarningFormat(obj, "{0} is empty - {1}({2})", fieldInfo.Name, obj.GetType().Name, obj.name);
                return false;
            }
        }
    }

    /// <summary>
    /// Specifies the minimum number of elements in an array field
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class MinSize : Attribute, ISceneCheckerAttribute
    {
        private int _size = 0;
        public MinSize(int size)
        {
            _size = size;
        }

        public bool CheckField(UnityEngine.Object obj, FieldInfo fieldInfo)
        {
            var value = fieldInfo.GetValue(obj);
            if (value is IEnumerable)
            {
                IEnumerable enumerableObj = (IEnumerable)value;
                int count = 0;
                foreach(object o in enumerableObj)
                {
                    count++;
                }
                if(count < _size)
                {
                    Debug.LogWarningFormat(obj, "{0} must have at least {3} elements {1}({2})", fieldInfo.Name, obj.GetType().Name, obj.name, _size);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else{
                Debug.LogWarningFormat(obj, "{0} must have at least {3} elements {1}({2})", fieldInfo.Name, obj.GetType().Name, obj.name, _size);
                return false;
            }
        }
    }

    /// <summary>
    /// Specifies that a game object or monobehaviour reference must point to something active in the scene hierarchy and cannot be null
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ActiveObjectsOnly : Attribute, ISceneCheckerAttribute
    {
        public bool CheckField(UnityEngine.Object obj, FieldInfo fieldInfo)
        {
            var value = fieldInfo.GetValue(obj);
            if (value is UnityEngine.Object)
            {
                UnityEngine.Object unityObj = (UnityEngine.Object)value;
                if (!unityObj)
                {
                    Debug.LogWarningFormat(obj, "'{0}' is null - {1}({2})", fieldInfo.Name, obj.GetType().Name, obj.name);
                    return false;
                }

                if (value is GameObject)
                {
                    GameObject go = (GameObject)value;
                    if (!go.activeInHierarchy)
                    {
                        Debug.LogWarningFormat(obj, "'{0}' is not active in the hierarchy - {1}({2})", fieldInfo.Name, obj.GetType().Name, obj.name);
                        return false;
                    }
                }

                if (value is Behaviour)
                {
                    Behaviour b = (Behaviour)value;
                    if (!b.gameObject.activeInHierarchy)
                    {
                        Debug.LogWarningFormat(obj, "'{0}' is not active in the hierarchy - {1}({2})", fieldInfo.Name, obj.GetType().Name, obj.name);
                        return false;
                    }
                }

            }
            

            if (value is IEnumerable)
            {
                IEnumerable enumerableObj = (IEnumerable)value;

                foreach (var elem in enumerableObj)
                {
                    UnityEngine.Object unityObj = (UnityEngine.Object)elem;
                    if (!unityObj)
                    {
                        Debug.LogWarningFormat(obj, "'{0}' contains null elements - {1}({2})", fieldInfo.Name, obj.GetType().Name, obj.name);
                        return false;
                    }

                    if (elem is GameObject)
                    {
                        GameObject go = (GameObject)elem;
                        if (!go.activeInHierarchy)
                        {
                            Debug.LogWarningFormat(obj, "'{0}' contains elements which are not active in the hierarchy - {1}({2})", fieldInfo.Name, obj.GetType().Name, obj.name);
                            return false;
                        }
                    }

                    if (elem is Behaviour)
                    {
                        Behaviour b = (Behaviour)elem;
                        if (!b.gameObject.activeInHierarchy)
                        {
                            Debug.LogWarningFormat(obj, "'{0}' contains elements which are not active in the hierarchy - {1}({2})", fieldInfo.Name, obj.GetType().Name, obj.name);
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
