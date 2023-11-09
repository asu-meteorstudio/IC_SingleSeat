using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artanim.SeatManagement
{
    public abstract class SeatLayout : MonoBehaviour
    {
        public List<Seat> _availableSeats = new List<Seat>();

        public GameObject _chaperoneVisualRoot;
        private List<SkinnedMeshRenderer> _skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
        private List<MeshRenderer> _meshRenderers = new List<MeshRenderer>();

        public Material chaperoneMaterial;
        private Material mat;

        void Awake()
        {
            SetChaperoneVisibility(false);

            //Only do chaperone if you aren't the professor
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
                }

                foreach (SkinnedMeshRenderer skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>(true))
                {
                    List<Material> newMaterials = new List<Material>();
                    foreach (Material m in skinnedMeshRenderer.materials)
                        newMaterials.Add(mat);
                    skinnedMeshRenderer.materials = newMaterials.ToArray();
                    _skinnedMeshRenderers.Add(skinnedMeshRenderer);
                }
            }
        }

        // Show or hide the chaperone geometry
        public void SetChaperoneVisibility(bool state)
        {
            if (_chaperoneVisualRoot) _chaperoneVisualRoot.SetActive(state);

            foreach(Seat seat in _availableSeats)
            {
                seat.SetChaperoneVisibility(state);
            }

            if (SeatManager.Instance._verbose)
                Debug.Log("[" + Time.frameCount + "] - setting pod chaperone visibility to " + state);
        }

        public void SetSeatLayoutColor(Color color)
        {
            mat.color = color;
        }
    }
}
