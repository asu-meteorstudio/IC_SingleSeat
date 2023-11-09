using Artanim;
using Artanim.Location.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VirtualTour
{
    public class TourManager : MonoBehaviour
    {
        [Header("Movement Variables")]
        public Transform GlobalOffset;
        public Transform Position1, Position2;
        public AnimationCurve curve;

        //MAF variables
        private AudioSource WakeUp;


        private Transform target;
        public bool transition;
        private int scene;

        // Start is called before the first frame update
        void Start()
        {
            scene = 0;
            target = null;
            WakeUp = GetComponent<AudioSource>();

            transform.position = Position1.position;            
        }

        // Update is called once per frame
        void Update()
        {
            if(transition)
            {
                transition = false;
                DoTransition();
            }
        }

        private void OnDestroy()
        {


        }

        public void DoTransition()
        {
            StartCoroutine(Animate());
        }

        private IEnumerator Animate()
        {
            float time = 12;
            float delta = 0;

            while(delta < 12)
            {
                delta += Time.deltaTime;
                transform.position = Vector3.Lerp(Position1.position, Position2.position, curve.Evaluate(delta / time));

                yield return null;
            }

            Potter();
        }

        private void Potter()
        {
            StartCoroutine(DoPotterTransition());
        }

        private IEnumerator DoPotterTransition()
        {
            if(NetworkInterface.Instance.IsClient)
            {
                VRCameraFader fader = FindObjectOfType<VRCameraFader>();
                StartCoroutine(fader.DoFadeAsync(Artanim.Location.Messages.Transition.FadeBlack));
            }

            //start loading in scene

            yield return new WaitForSeconds(0.25f);

            WakeUp.Play();

            //fake in
        }
    }
}

