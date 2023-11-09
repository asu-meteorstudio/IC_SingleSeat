using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VirtualClassroom
{
    public class CheckAnswersTrigger : MonoBehaviour
    {
        public bool debug_ToggleUI = false;
        public bool debug_CheckAnswers = false;
        
        public QuizzManager[] quizzManagers;

        bool toggleState = true;

        // Start is called before the first frame update
        void Start()
        {
            //quizzManagers = FindObjectsOfType<QuizzManager>();
            ToggleUI();
        }

        // Update is called once per frame
        public void CheckAnswers()
        {
            Debug.Log("Check Answers");
            foreach (var quizzManager in quizzManagers)
            {
                quizzManager.ValidateCurrentQuestion();
            }
        }

        // Update is called once per frame
        public void ToggleUI()
        {
            Debug.Log("Toggle UI");
            if (toggleState) toggleState = false;
            else toggleState = true;
            
            foreach (var quizzManager in quizzManagers)
            {
                quizzManager.gameObject.SetActive(toggleState);
            }
        }

        public void Update()
        {
            if (debug_ToggleUI)
            {
                ToggleUI();
                debug_ToggleUI = false;
            }

            if (debug_CheckAnswers)
            {
                CheckAnswers();
                debug_CheckAnswers = false;
            }
        }
    }
}
