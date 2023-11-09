using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artanim.SeatManagement
{
    public class Statue_Switcher : SingletonBehaviour<Statue_Switcher>
    {
        public List<GameObject> statues;

        private GameObject currentStatue;

        private int index;

        public float rotationSpeed;

        public bool debugNext = false;

        public bool debugPrevious = false;

        public int GetCurrentStatueIndex()
        {
            return index;
        }

        public void NextStatue()
        {
            if (currentStatue)
                GameObject.Destroy(currentStatue);

            if (index < statues.Count) index++;

            currentStatue = Instantiate(statues[index], transform.position, Quaternion.identity, transform);
        }

        public void PreviousStatue()
        {
            if (currentStatue)
                GameObject.Destroy(currentStatue);

            if (index > 0) index--;

            currentStatue = Instantiate(statues[index], transform.position, Quaternion.identity, transform);
        }

        private void Start()
        {
            currentStatue = Instantiate(statues[0], transform.position, Quaternion.identity, transform);
        }

        private void Update()
        {
            transform.Rotate(0f, Time.deltaTime * rotationSpeed, 0f);

            if (debugNext)
            {
                NextStatue();
                debugNext = false;
            }
            if (debugPrevious)
            {
                PreviousStatue();
                debugPrevious = false;
            }
        }
    }

}
