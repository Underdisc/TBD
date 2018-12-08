using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        timeElapsed += Time.deltaTime;
        stage();
    }
}
