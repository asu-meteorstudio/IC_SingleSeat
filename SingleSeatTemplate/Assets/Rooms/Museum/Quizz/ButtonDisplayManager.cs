using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Artanim;

namespace VirtualClassroom
{
    public class ButtonDisplayManager : MonoBehaviour
    {
        public PropositionID id;

        public QuestionDisplayManager questionManager;
        public Image outline;
        public Color baseOutlineColor;

        public Image background;
        public Color baseBackgroundColor;

        public TextMeshProUGUI text;

        public bool isSelected = false;

        public delegate void ButtonSelected(PropositionID id);
        public event ButtonSelected OnPropositionSelected;

        public bool debugTrigger = false;

        // Start is called before the first frame update
        void Start()
        {
            baseOutlineColor = outline.color;
            baseBackgroundColor = background.color;
        }

        // Update is called once per frame
        void Update()
        {
            if (debugTrigger)
            {
                OnPropositionSelected?.Invoke(id);
                debugTrigger = false;
            }
        }

        public void OnHandEnter()
        {
            // Notify the quizz manager that this button was selected
            OnPropositionSelected?.Invoke(id);
        }
    }
}
