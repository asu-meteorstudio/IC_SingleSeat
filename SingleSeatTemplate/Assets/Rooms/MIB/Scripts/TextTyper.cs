using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextTyper : MonoBehaviour {

    private Text typedText;
    public float timeStep;
    public float timeBetweenLines;
    private string allText;
    private int textIndex;
    private bool canDisplayText = true;

	// Use this for initialization
	void Start () {
        //typedText = GetComponent<Text>();
        //allText = typedText.text;
        //typedText.text = "";
        //if (canDisplayText)
        //    StartTextType();
    }

    private void OnEnable()
    {
        typedText = GetComponent<Text>();
        allText = typedText.text;
        typedText.text = "";
        if (canDisplayText)
            StartTextType();
    }

    //private void OnDisable()
    //{
    //    canDisplayText = true;
    //}

    public void StartTextType()
    {
        
        StartCoroutine(TypeText());
    }

    public IEnumerator TypeText()
    {
        canDisplayText = false;
        if (allText[textIndex] == '\n')
        {
            yield return new WaitForSeconds(timeBetweenLines);
        }
        else
        {
            yield return new WaitForSeconds(timeStep);
        }
        typedText.text = typedText.text + allText[textIndex];
        textIndex++;
        
        if (textIndex < allText.Length)
            StartCoroutine(TypeText());
        canDisplayText = true;
    }
}
