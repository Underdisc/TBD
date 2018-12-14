using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuIntro : MonoBehaviour 
{


    public GameObject prompt;
    public float promptFadeInTime;
    [Range(0.0f, 1.0f)]
    public float promptSinIntensity;
    public float promptSinFrequency;

    public GameObject[] movers;
    private List<Transform> moverTrans;
    public GameObject[] endPositionObjects;
    private List<Vector3> startPositions;
    private List<Vector3> endPositions;
    private List<Vector3> deltaPositions;

    public float transitionTime;

    private float timeElapsed;

    private delegate void VoidVoidFunction();
    private VoidVoidFunction stage;

    public Button startButton;
    public Button howToPlayButton;
    public Button exitButton;
    public Button backButton;

    public GameObject buttons;
    public Transform buttonsOn;
    public Transform buttonsOff;
    public GameObject howToPlay;
    public Transform howToPlayOn;
    public Transform howToPlayOff;
    public float howToPlayTransitionTime;

    private bool performingAction = true;

    void LerpTo(GameObject thing, Transform start_trans, Transform end_trans, float perc)
    {
        Vector3 start = start_trans.position;
        Vector3 end = end_trans.position;
        Vector3 delta = end - start;
        Vector3 pos = start + delta * perc;
        thing.transform.position = pos;
    }

    void HowToPlayTransition()
    {
        if(timeElapsed > howToPlayTransitionTime)
        {
            howToPlay.transform.position = howToPlayOn.position;
            buttons.transform.position = buttonsOff.position;
            performingAction = false;
        }
        float perc = timeElapsed / howToPlayTransitionTime;
        perc = ActionOperation.QuadOutIn(perc);
        LerpTo(buttons, buttonsOn, buttonsOff, perc);
        LerpTo(howToPlay, howToPlayOff, howToPlayOn, perc);
    }

    void BackTransition()
    {
        if(timeElapsed > howToPlayTransitionTime)
        {
            howToPlay.transform.position = howToPlayOff.position;
            buttons.transform.position = buttonsOn.position;
            performingAction = false;
        }
        float perc = timeElapsed / howToPlayTransitionTime;
        perc = ActionOperation.QuadOutIn(perc);
        LerpTo(buttons, buttonsOff, buttonsOn, perc);
        LerpTo(howToPlay, howToPlayOn, howToPlayOff, perc);
    }

    void OnBack()
    {
        stage = BackTransition;
        performingAction = true;
        timeElapsed = 0.0f;
    }

    void OnStart()
    {
        SceneManager.LoadScene("Arena");
    }

    void OnHowToPlay()
    {
        stage = HowToPlayTransition;
        performingAction = true;
        timeElapsed = 0.0f;
    }

    void OnExit()
    {
        Application.Quit();
    }


    void Start () 
    {
        timeElapsed = 0.0f;
        stage = PromptStage;
        moverTrans = new List<Transform>();
        startPositions = new List<Vector3>();
        endPositions = new List<Vector3>();
        deltaPositions = new List<Vector3>();
        foreach(GameObject mover in movers)
        {
            Transform trans = mover.GetComponent<Transform>();
            moverTrans.Add(trans);
            startPositions.Add(trans.position);
        }

        foreach(GameObject endPositionObject in endPositionObjects)
        {
            Transform trans = endPositionObject.GetComponent<Transform>();
            endPositions.Add(trans.position);
        }

        for(int i = 0; i < startPositions.Count; ++i)
        {
            Vector3 start = startPositions[i];
            Vector3 end = endPositions[i];
            deltaPositions.Add(end - start);
        }
    }

    void TransitionStage()
    {
        timeElapsed += Time.deltaTime;
        if(timeElapsed > transitionTime)
        {
            for(int i = 0; i < moverTrans.Count; ++i)
            {
                moverTrans[i].position = endPositions[i];
            }
            startButton.onClick.AddListener(OnStart);
            howToPlayButton.onClick.AddListener(OnHowToPlay);
            exitButton.onClick.AddListener(OnExit);
            backButton.onClick.AddListener(OnBack);
            performingAction = false;
            return;
        }

        float perc = timeElapsed / transitionTime;
        perc = ActionOperation.QuadOutIn(perc);
        for(int i = 0; i < moverTrans.Count; ++i)
        {
            Transform mover_trans = moverTrans[i];
            Vector3 start = startPositions[i];
            Vector3 delta = deltaPositions[i];
            mover_trans.position = start + delta * perc;
        }
        
    }

    void PromptStage()
    {
        float fade_perc = timeElapsed / promptFadeInTime;
        fade_perc = Mathf.Min(1.0f, fade_perc);
        float omega = timeElapsed * 2.0f * Mathf.PI * promptSinFrequency;
        float sin_perc = Mathf.Sin(omega);
        sin_perc = (sin_perc / 2.0f) + 1.0f;
        sin_perc = 1.0f - sin_perc * promptSinIntensity;
        float transparency = sin_perc * fade_perc;

        Text text_comp = prompt.GetComponent<Text>();
        Vector4 color = text_comp.color;

        if(Input.GetKey(KeyCode.Space))
        {
            timeElapsed = 0.0f;
            color.w = 1.0f;
            stage = TransitionStage;
        }
        else
        {
            color.w = transparency;
        }

        text_comp.color = color;
    }
    
    void Update () 
    {
        if(performingAction)
        {
            timeElapsed += Time.deltaTime;
            stage();
        }
    }
}
