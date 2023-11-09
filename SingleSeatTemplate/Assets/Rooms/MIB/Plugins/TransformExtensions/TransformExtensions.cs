using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static partial class TransformExtensions
{
    //public static Transform FindClosestTransform(this Transform mainTransform, params Transform[] array)
    //{
    //    return FindClosestTransform(mainTransform, array);
    //}

    public static Transform FindClosestTransform<T>(this Transform mainTransform, IEnumerable<T> array) where T : Component
    {
        return mainTransform.FindClosestComponent(array).transform;
    }

    public static T FindClosestComponent<T>(this Transform mainTransform, IEnumerable<T> array) where T : Component
    {
        return array.OrderBy(c => (c.transform.position - mainTransform.position).sqrMagnitude).FirstOrDefault();
    }

    public static Transform FindClosestTransform(this Transform mainTransform, IEnumerable<Transform> array)
    {
        return array.OrderBy(target => (target.position - mainTransform.position).sqrMagnitude).FirstOrDefault();
    }

    public static string GetPath<T>(this T obj) where T : Component
    {
        return obj.transform.GetPath();
    }

    public static string GetPath(this Transform obj)
    {
        string path = "/" + obj.name;
        while (obj.parent != null)
        {
            obj = obj.parent;
            path = "/" + obj.name + path;
        }
        return path;
    }

    /// <summary>
    /// Align some child's rotation and position to a source by rotating and translating the parent.
    /// Note that target and targetChild don't require a direct parent/child relationship, but can be any number of levels of ancestor/descendant in the hierachy
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="child"></param>
    /// <param name="destination"></param>
    public static void Align(this Transform parent, Transform child, Transform destination)
    {
        parent.rotation = destination.rotation * Quaternion.Inverse(Quaternion.Inverse(parent.rotation) * child.rotation);
        parent.position = destination.position + (parent.position - child.position);
    }
}
