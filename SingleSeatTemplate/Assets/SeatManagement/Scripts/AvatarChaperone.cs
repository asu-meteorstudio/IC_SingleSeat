using Artanim.Location.Network;
using System.Collections.Generic;
using UnityEngine;

namespace Artanim.SeatManagement
{
    [RequireComponent(typeof(Animator))]
    public class AvatarChaperone : MonoBehaviour
    {
        private Dictionary<SkinnedMeshRenderer, SkinnedMeshRenderer> RendererMap = new Dictionary<SkinnedMeshRenderer, SkinnedMeshRenderer>();
        private List<SkinnedMeshRenderer> _skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
        private List<MeshRenderer> _meshRenderers = new List<MeshRenderer>();

        //Allow inject or AvatarController for chaperone avatars
        public AvatarController SourceAvatarController;
        public Material chaperoneMaterial;

        public bool _visibilityToggle = false;
        private bool _previousVisibilityToggle;

        private Animator SourceAnimator;
        private Animator _TargetAnimator;
        private Animator TargetAnimator
        {
            get
            {
                if (!_TargetAnimator)
                    _TargetAnimator = GetComponent<Animator>();
                return _TargetAnimator;
            }
        }

        private Material mat;

        private bool _isServer;

        void Start()
        {
            // In case we are in ClientServer or Standalone mode, act as a server
            if (DevelopmentMode.CurrentMode != EDevelopmentMode.None)
            {
                Debug.Log("[" + Time.frameCount + "] -Development Mode detected - Initiating the avatar chaperone as if we were a server");
                _isServer = true;
            }
            else // Normal mode
            {
                _isServer = NetworkInterface.Instance.IsServer;
            }

            //Source avatar animator
            if (!SourceAvatarController)
            SourceAvatarController = GetComponentInParent<AvatarController>();

            if (SourceAvatarController)
                SourceAnimator = SourceAvatarController.AvatarAnimator;

            //Blendshape map
            var sourceFaceController = SourceAvatarController.GetComponent<AvatarFaceController>();
            if (sourceFaceController)
            {
                foreach (var sourceRenderer in sourceFaceController.FaceRenderers)
                {
                    var targetObject = UnityUtils.GetChildByName(sourceRenderer.name, transform);
                    if (targetObject)
                    {
                        var targetRenderer = targetObject.GetComponent<SkinnedMeshRenderer>();
                        if (targetRenderer)
                        {
                            RendererMap.Add(sourceRenderer, targetRenderer);
                        }
                        else
                        {
                            Debug.LogError("Failed to find target renderer for: " + sourceRenderer.name);
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to find target object for: " + sourceRenderer.name);
                    }
                }
            }

            if (chaperoneMaterial)
            {
                mat = new Material(chaperoneMaterial);
                mat.color = Color.cyan;

                foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>(true))
                {
                    List<Material> newMaterials = new List<Material>();
                    foreach (Material m in meshRenderer.materials)
                        newMaterials.Add(mat);
                    meshRenderer.materials = newMaterials.ToArray();
                    _meshRenderers.Add(meshRenderer);
                    meshRenderer.enabled = false;
                }

                foreach (SkinnedMeshRenderer skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>(true))
                {
                    List<Material> newMaterials = new List<Material>();
                    foreach (Material m in skinnedMeshRenderer.materials)
                        newMaterials.Add(mat);
                    skinnedMeshRenderer.materials = newMaterials.ToArray();
                    _skinnedMeshRenderers.Add(skinnedMeshRenderer);
                    skinnedMeshRenderer.enabled = false;
                }
            }
            else
                Debug.LogError("Could not find a default chaperoneMaterial on " + gameObject.name);

            _previousVisibilityToggle = _visibilityToggle;
        }

        void OnEnable()
        {
            MirrorAvatar();
        }

        void Update()
        {
            if (SourceAvatarController)
                MirrorAvatar();

            if (_visibilityToggle != _previousVisibilityToggle)
            {
                SetAvatarVisibility(_visibilityToggle);
                _previousVisibilityToggle = _visibilityToggle;
            }
        }

        public void SetAvatarVisibility(bool state)
        {
            foreach (MeshRenderer meshRenderer in _meshRenderers)
            {
                meshRenderer.enabled = state;
            }

            foreach (SkinnedMeshRenderer skinnedMeshRendererd in _skinnedMeshRenderers)
            {
                skinnedMeshRendererd.enabled = state;
            }
        }

        public void SetAvatarColor(Color color)
        {
            mat.color = color;
        }

        private void MirrorAvatar()
        {
            //Mirror bones
            if (TargetAnimator && SourceAnimator)
            {
                for (var i = 0; i < (int)HumanBodyBones.LastBone; ++i)
                {
                    var sourceTransform = SourceAnimator.GetBoneTransform((HumanBodyBones)i);
                    var targetTransform = TargetAnimator.GetBoneTransform((HumanBodyBones)i);

                    targetTransform.localPosition = sourceTransform.localPosition;
                    targetTransform.localRotation = sourceTransform.localRotation;
                }

                TargetAnimator.transform.localPosition = SourceAnimator.transform.localPosition;
                TargetAnimator.transform.localRotation = SourceAnimator.transform.localRotation;
                TargetAnimator.transform.localScale = SourceAnimator.transform.localScale;
            }

            //Mirror blendshapes
            foreach (var map in RendererMap)
            {
                for (var i = 0; i < map.Key.sharedMesh.blendShapeCount; ++i)
                {
                    map.Value.SetBlendShapeWeight(i, map.Key.GetBlendShapeWeight(i));
                }
            }
        }

    }
}
