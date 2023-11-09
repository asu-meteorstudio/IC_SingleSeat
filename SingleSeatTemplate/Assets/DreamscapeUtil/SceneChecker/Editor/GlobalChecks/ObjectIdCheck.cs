#if DSUTIL_ARTANIM_COMMON_IN_PROJECT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Artanim;
using Artanim.Tracking;

namespace DreamscapeUtil
{
    [Description("Checks for duplicate or empty ObjectIds in Artanim scripts")]
    public class ObjectIdCheck : ISceneCheckerGlobalSceneCheck
    {
        public int CheckForErrors(SceneCheckerContext context)
        {
            int numErrors = 0;

            HashSet<string> objectIds = new HashSet<string>();
            HashSet<string> rigidBodyNames = new HashSet<string>();
            HashSet<string> offsetIds = new HashSet<string>();
            HashSet<string> networkSyncedTransformIds = new HashSet<string>();

            SceneCheckerIterator it = SceneCheckerIterator.IterContext(context);

            while (it.MoveNext())
            {
                GameObject go = it.Current;

                if (go.TryGetComponent<NetworkSyncedBehaviour>(out NetworkSyncedBehaviour nsb))
                {
                    if(string.IsNullOrEmpty(nsb.ObjectId))
                    {
                        numErrors++;
                        Debug.LogWarningFormat(nsb, "ObjectId cannot b empty - ({0})", nsb.name);
                    }
                    else
                    {
                        if (objectIds.Contains(nsb.ObjectId))
                        {
                            numErrors++;
                            Debug.LogWarningFormat(nsb, "Duplicate Object Id: {0}", nsb.ObjectId);
                        }
                        else{
                            objectIds.Add(nsb.ObjectId);
                        }
                    }

                    
                }
                if (go.TryGetComponent<TrackingRigidbody>(out TrackingRigidbody trb))
                {
                    if (string.IsNullOrEmpty(trb.RigidbodyName))
                    {
                        numErrors++;
                        Debug.LogWarningFormat(trb, "Rigidbody Name cannot be empty - ({0})", trb.name);
                    }
                    else
                    {
                        if (rigidBodyNames.Contains(trb.RigidbodyName))
                        {
                            numErrors++;
                            Debug.LogWarningFormat(trb, "Duplicate Rigidbody Name: {0}", trb.RigidbodyName);
                        }
                        rigidBodyNames.Add(trb.RigidbodyName);
                    }
                }
                if (go.TryGetComponent<AvatarOffset>(out AvatarOffset offset))
                {
                    if (string.IsNullOrEmpty(offset.ObjectId))
                    {
                        numErrors++;
                        Debug.LogWarningFormat(offset, "Object Id cannot be empty - ({0})", offset.gameObject.name);
                    }
                    else
                    {
                        if (offsetIds.Contains(offset.ObjectId))
                        {
                            numErrors++;
                            Debug.LogWarningFormat(offset, "Duplicate Object Id: {0}", offset.ObjectId);
                        }
                        offsetIds.Add(offset.ObjectId);
                    }
                }
                if (go.TryGetComponent<NetworkSyncedTransform>(out NetworkSyncedTransform nst))
                {
                    if (string.IsNullOrEmpty(nst.ObjectId))
                    {
                        numErrors++;
                        Debug.LogWarningFormat(nst, "Object Id cannot be empty - ({0})", nst.gameObject.name);
                    }
                    else
                    {
                        if (networkSyncedTransformIds.Contains(nst.ObjectId))
                        {
                            numErrors++;
                            Debug.LogWarningFormat(nst, "Duplicate Object Id: {0}", nst.ObjectId);
                        }
                        networkSyncedTransformIds.Add(nst.ObjectId);
                    }
                }

            }

            return numErrors;
        }
    }
}
#endif
