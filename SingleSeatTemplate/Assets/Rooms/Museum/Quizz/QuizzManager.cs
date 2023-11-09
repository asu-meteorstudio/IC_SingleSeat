using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artanim.SeatManagement;
using Artanim;

namespace VirtualClassroom
{
    public class QuizzManager : MonoBehaviour
    {
        public bool isProfessorQuizz = false;
        
        public VirtualSeat virtualSeat;
        
        public List<QuestionDisplayManager> questions;

        int currentQuestion = 0;
        public bool nextQuestionTrigger = false;

        bool isInitialized = false;

        public void Awake()
        {
            Init();
        }

        void Init()
        {
            if (virtualSeat == null || isProfessorQuizz)
            { 
                gameObject.SetActive(false);
            }
            else
            {
                int i = 0;
                foreach (var question in questions)
                {
                    AvatarTrigger[] triggers = question.gameObject.GetComponentsInChildren<AvatarTrigger>();
                    foreach (var trigger in triggers)
                    {
                        trigger.ObjectId = trigger.ObjectId + virtualSeat.gameObject.name + "_Q" + i;
                        //Debug.Log("ObjectId: " + trigger.ObjectId);

                    }

                    question.gameObject.SetActive(false);

                    i++;
                }
            }

            isInitialized = true;
        }

        void Update()
        {
            if (!isInitialized)
                Init();
            
            if (isProfessorQuizz || !virtualSeat._isAvailable)
            {
                currentQuestion = Statue_Switcher.Instance.GetCurrentStatueIndex();
                DisplayCurrentQuestion();

                if (nextQuestionTrigger)
                {
                    currentQuestion++;
                    DisplayCurrentQuestion();
                    nextQuestionTrigger = false;
                }
            }
        }

        void DisplayCurrentQuestion()
        {
            for (int i = 0; i < questions.Count; i++)
            {
                if (i == currentQuestion)
                    questions[i].gameObject.SetActive(true);
                else
                    questions[i].gameObject.SetActive(false);
            }
        }

        public void ValidateCurrentQuestion()
        {
            questions[currentQuestion].ValidateQuestion();
        }
    }
}
