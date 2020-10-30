using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneMetaballs : MonoBehaviour
{
    public GameObject Node;
    public Material Mat;
    public Transform Cam;
    public Transform Cameras;
    public GameObject NextScene;
    public AudioSource Music;
    public RawImage Render;

    private Metaballs _Metaballs = new Metaballs();

    private float _Timer = 0.0f;
    private int _CamID = 0;

    void Start()
    {
        _Metaballs.Initialize(Node, Mat);
    }

    void Update()
    {
        _Timer += Time.deltaTime;
        
        if (SoundController.Get().MidiBassThree)
        {
            _Timer = 0.0f;
            _CamID = (_CamID+1)%Cameras.childCount;    
            SoundController.Get().Pixelate();
        }

        float lerp = _Timer*1.0f;

        Cam.position = Vector3.Lerp(Cameras.GetChild(_CamID).position, Cameras.GetChild(_CamID).GetChild(0).position, lerp);
        Cam.rotation = Quaternion.Lerp(Cameras.GetChild(_CamID).rotation, Cameras.GetChild(_CamID).GetChild(0).rotation, lerp);

        _Metaballs.Update(_Timer*1000.0f);

        if (Music.time>=182.0f)
        {
            gameObject.SetActive(false);
            NextScene.SetActive(true);
        }

        Render.material.SetFloat("_DeformSquare", Mathf.MoveTowards(Render.material.GetFloat("_DeformSquare"), 0.0f, Time.deltaTime*0.2f));
        Render.material.SetFloat("_GlitchStrength", Mathf.MoveTowards(Render.material.GetFloat("_GlitchStrength"), 0.0f, Time.deltaTime*0.2f));
    }
}
