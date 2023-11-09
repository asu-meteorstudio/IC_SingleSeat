using Artanim;
using Artanim.Location.Data;
using Artanim.Location.Hostess;
using Artanim.Location.SharedData;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

/// <summary>
/// see here https://lightbuzz.com/speech-recognition-unity/
/// </summary>
public class SpeechRecognitionEngine : ClientSideBehaviour
{
    public string[] keywords = new string[] { "start" };
    public ConfidenceLevel confidence = ConfidenceLevel.Medium;

    //public Text results;
    //public Image target;

    protected PhraseRecognizer recognizer;
    protected string word = "right";
    private Session ThisSession;

    private void Awake()
    {
        if(!SharedDataUtils.GetMyComponent<LocationComponent>().PodId.Contains("PROFESSOR"))
        {
            gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if (keywords != null)
        {
            recognizer = new KeywordRecognizer(keywords, confidence);
            recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
            recognizer.Start();
        }

        foreach (var device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
        }
    }

    private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        word = args.text;
        //results.text = "You said: <b>" + word + "</b>";
    }

    private void Update()
    {
        //var x = target.transform.position.x;
        //var y = target.transform.position.y;

        switch (word)
        {
            case "start":
                Debug.Log("You said: " + word);
                word = "";
                //Session session = GameController.Instance.CurrentSession;
                //session.StartScene = "classroom";
                //StartSessionStatus result = SessionManager.StartSession(session);

                break;
        }
    }

    private void OnDestroy()
    {
        if (recognizer != null && recognizer.IsRunning)
        {
            recognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
            recognizer.Stop();
        }
    }

    private void OnApplicationQuit()
    {
        if (recognizer != null && recognizer.IsRunning)
        {
            recognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
            recognizer.Stop();
        }
    }
}
