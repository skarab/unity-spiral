using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SceneTitle : MonoBehaviour
{
    public Material RenderMat;
    public Transform Cam;
    public Transform Cameras;
    public AudioSource Music;
    public VisualEffect Particles;  
    public GameObject NextScene;

    private float _Timer = 0.0f;
    private int _CamID = 0;
    private float _BassTimer = 4.0f;

    void Start()
    {
        RenderMat.SetFloat("_Deform", 0.0f);
        
        RenderUI.Get().Clear();
        RenderUI.Get().EnableTV(false);

        Particles.SetFloat("Cut", 0.0f);
    }

    void Update()
    {
        _Timer += Time.deltaTime;
        
        if (Music.time>=105.0f)
            Particles.SetFloat("Cut", Mathf.MoveTowards(Particles.GetFloat("Cut"), 1.0f, Time.deltaTime*0.2f));
    
        _BassTimer -= Time.deltaTime;
        if (_BassTimer<=0.0f)
        {
            _Timer = 0.0f;
            _CamID = (_CamID+1)%Cameras.childCount;
            _BassTimer = 4.0f;
        }

        if (SoundController.Get().MidiBass)
            _BassTimer = 0.2f;

        float lerp = _Timer*4.0f;

        Cam.position = Vector3.Lerp(Cameras.GetChild(_CamID).position, Cameras.GetChild(_CamID).GetChild(0).position, lerp);
        Cam.rotation = Quaternion.Lerp(Cameras.GetChild(_CamID).rotation, Cameras.GetChild(_CamID).GetChild(0).rotation, lerp);

        if (Music.time>=115.0f)
        {
            gameObject.SetActive(false);
            NextScene.SetActive(true);
        }         
    }
}
