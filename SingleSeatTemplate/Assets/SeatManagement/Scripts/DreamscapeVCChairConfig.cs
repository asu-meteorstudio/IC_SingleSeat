using Artanim.Location.Data;
using Artanim.Tracking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Artanim
{
    public class DreamscapeVCChairConfig : BaseSeatedExperienceChairConfig
    {
        [Header("VC chair settings")]
        public Transform InitialChairRoot;
        public Transform ChairPivot;

        public List<string> SceneWithoutChairVisuals;

        private float rootPositionSmoothingFactor = 0.0025f;
        private bool playerAssigned = false;
        private bool resetRootPosition = true;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public override void EstimateChairRootTransform(ECalibrationMode calibrationMode, Vector3 chairTrackerPosition, Quaternion chairTrackerRotation, out Vector3 chairRootPosition, out Quaternion chairRootRotation)
        {
            //Apply tracking to be sure it's up to date
            SeatTracker.transform.SetPositionAndRotation(chairTrackerPosition, chairTrackerRotation);

            chairRootPosition = new Vector3(ChairPivot.position.x, 0f, ChairPivot.position.z);
            
            chairRootRotation = new Quaternion();
            chairRootRotation.eulerAngles = new Vector3(0f, ChairPivot.rotation.eulerAngles.y, 0f);

            //Apply calculated root to local model
            RootTarget.position = chairRootPosition;
            RootTarget.rotation = chairRootRotation;
        }

        public override void AssignPlayer(SkeletonConfig skeleton)
        {
            var pelvisName = skeleton.SkeletonSubjectNames[(int)ESkeletonSubject.Pelvis];
            if (SeatTracker)
            {
                Debug.Log("Assinging player to " + pelvisName);
                SeatTracker.ResetRigidbodyName(pelvisName);
                playerAssigned = true;
            }

            /*var skeletonPostfix = pelvisName.Substring(pelvisName.IndexOf("_") + 1);
            if (HandleTracker)
                HandleTracker.ResetRigidbodyName(string.Format(HANDLE_PATTERN, skeletonPostfix));*/
        }

        private void SetChairVisibility(bool isVisible)
        {
            foreach (var visual in ChairVisuals)
            {
                var meshRenderer = visual.GetComponent<MeshRenderer>();
                meshRenderer.enabled = isVisible;
            } 
        }

        public void Update()
        {
            if (playerAssigned)
            {
                if (resetRootPosition)
                {
                    RootTarget.position = new Vector3(ChairPivot.position.x, 0f, ChairPivot.position.z);
                    resetRootPosition = false;
                }
                else
                { 
                    RootTarget.position = new Vector3(
                        Mathf.Lerp(RootTarget.position.x, ChairPivot.position.x, rootPositionSmoothingFactor),
                        0f,
                        Mathf.Lerp(RootTarget.position.z, ChairPivot.position.z, rootPositionSmoothingFactor)
                    );
                }

                //Set local height to zero so that the chair base is always on the floor even when there are global/avatar offsets
                RootTarget.localPosition = new Vector3(RootTarget.localPosition.x, 0f, RootTarget.localPosition.z);
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (SceneWithoutChairVisuals.Contains(scene.name))
            {
                SetChairVisibility(false);
            }
            else
            {
                SetChairVisibility(true);
            }
        }
    }
}