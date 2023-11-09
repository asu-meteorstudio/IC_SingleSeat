using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VirtualClassroom
{
    public enum PropositionID
    {
        None,
        A,
        B,
        C,
        D
    }

    public class QuestionDisplayManager : MonoBehaviour
    {
        public TextMeshProUGUI question;
        public Image question_Frame;
        public List<ButtonDisplayManager> propositions;

        Color selectedPropositionColor = Color.white;
        Color rightColor = new Color(0, 1, 0, 0.25f);
        Color wrongColor = new Color(1, 0, 0, 0.25f);

        PropositionID selectedPropositionId = PropositionID.None;
        public PropositionID correctPropositionId = PropositionID.B;

        public bool validateQuestionTrigger = false;

        void OnEnable()
        {
            foreach (var proposition in propositions)
                proposition.OnPropositionSelected += OnPropositionSelected;
        }

        void OnDisable()
        {
            foreach (var proposition in propositions)
                proposition.OnPropositionSelected -= OnPropositionSelected;
        }

        void Update()
        {
            // DEBUG
            if (validateQuestionTrigger)
            {
                ValidateQuestion();
                validateQuestionTrigger = false;
            }
        }

        public void ValidateQuestion()
        {
            for (int i = 0; i < propositions.Count; i++)
            {
                if (propositions[i].id == correctPropositionId)
                {
                    // Student selected the correct proposition
                    if (selectedPropositionId == propositions[i].id)
                    {
                        propositions[i].outline.color = rightColor;
                        question_Frame.color = rightColor;
                        // TODO: store positive results
                    }
                    // Student did not select the correct proposition
                    else
                    {
                        propositions[i].outline.color = rightColor;
                        question_Frame.color = wrongColor;
                    }
                }
                else if (selectedPropositionId == propositions[i].id)
                {
                    // Student selected a wrong proposition
                    propositions[i].outline.color = wrongColor;
                }
            }
        }

        void OnPropositionSelected(PropositionID id)
        {
            selectedPropositionId = id;

            for (int i = 0; i < propositions.Count; i++)
            { 
                if (propositions[i].id == id)
                    propositions[i].outline.color = selectedPropositionColor;
                else
                    propositions[i].outline.color = propositions[i].baseOutlineColor;
            }

            IEnumerator coroutine = ValidateAnswerAfterDelay(2f);
            StartCoroutine(coroutine);
        }

        IEnumerator ValidateAnswerAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            validateQuestionTrigger = true;
        }
    }
}
