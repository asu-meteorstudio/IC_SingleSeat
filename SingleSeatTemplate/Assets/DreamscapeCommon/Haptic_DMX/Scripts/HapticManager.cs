using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artanim.Location.Network;
using Artanim;
using UnityEngine.Audio;
using Artanim.Location.SharedData;
using Artanim.Location.Data;

namespace Dreamscape
{
    public class HapticManager : ServerSideBehaviour
    {
        private static HapticManager _manager;
        public static HapticManager Instance
        {
            get
            {
                return _manager;
            }
        }

        [Header("Buttkickers")]
        public AudioMixerSnapshot ServerSnapshot;
        public AudioSource rumble;

        [Header("Fan Variables")]
        public GameObject fansParent;
        public GameObject deskFanParent;
        private DMX_fan[] fans;
        private DMX_fan[] deskfans;
        private bool updatingFanSpeed;

        [Header("Scent Variables")]
        public DMX_scent[] scent1;
        public DMX_scent[] scent2;
        //private bool scent1Playing;
        //private bool scent2Playing;

        [Header("Spray Variables")]
        public GameObject mistParent;
        private DMX_mist[] mist;
        private bool allMistPlaying;

        private string podid;
        public float m_fanspeed;

        void Start()
        {

            if (ServerSnapshot)
                ServerSnapshot.TransitionTo(0);
            mist = mistParent.GetComponentsInChildren<DMX_mist>();
            fans = fansParent.GetComponentsInChildren<DMX_fan>();
            deskfans = deskFanParent.GetComponentsInChildren<DMX_fan>();
            _manager = this;

            m_fanspeed = 0;
        }

        public void StartButtkickerRumble()
        {
            if (rumble)
                rumble.Play();
        }

        public void UpdateAllFans(float fanSpeed)
        {
            podid = SharedDataUtils.GetMyComponent<LocationComponent>().PodId;
            m_fanspeed = fanSpeed;

            if (podid.Contains("GSV") || podid.Contains("ASU00001-04") || podid.Contains("COMBO") || podid.Contains("ASU00001-03"))
            {
                foreach (DMX_fan fan in deskfans)
                    fan.speed = fanSpeed;
            }
            else
            { //assuing that the alternative is a full pod
                foreach (DMX_fan fan in fans)
                    fan.speed = fanSpeed;
            }
        }

        public void UpdateFansOverTime(float speed, float time)
        {
            if (!updatingFanSpeed)
            {
                updatingFanSpeed = true;
                StartCoroutine(FanSpeedLerp(speed, time));
            }

        }

        /// <summary>
        /// Changes all fans to a certain speed over time
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        protected IEnumerator FanSpeedLerp(float speed, float time)
        {
            float timer = 0;
            float currentFanSpeed = fans[0].speed;
            while (timer <= time)
            {
                foreach (DMX_fan fan in fans)
                    fan.speed = Mathf.Lerp(currentFanSpeed, speed, timer / time);

                timer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            updatingFanSpeed = false;
        }

        public void SprayMistAll(float totalTime)
        {
            if (!allMistPlaying)
            {
                StartCoroutine(AllMistersOn(totalTime));
            }
        }

        protected IEnumerator AllMistersOn(float timeTotal)
        {
            allMistPlaying = true;
            float time = 0;
            foreach (DMX_mist spray in mist)
                spray.speed = 1;
            while (time < timeTotal)
            {
                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            foreach (DMX_mist spray in mist)
                spray.speed = 0;
            allMistPlaying = false;
        }

    }
}

