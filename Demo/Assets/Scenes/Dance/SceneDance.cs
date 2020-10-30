using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDance : MonoBehaviour
{
    public AudioSource Music;
    public Transform Cam;
    public Transform Cameras;
    public Light LightOne;
    public Light LightTwo;
    public GameObject NextScene;

    private float _Timer = 0.0f;
    private int _CamID = 0;

    void Start()
    {
        LightTwo.intensity = 1e+14f;
        LightOne.intensity = 1e+14f;
    }

    void Update()
    {
        _Timer += Time.deltaTime;
        
        if (SoundController.Get().MidiBassThree)
        {
            _Timer = 0.0f;
            _CamID = (_CamID+1)%Cameras.childCount;
        }

        LightTwo.intensity = Mathf.Lerp(LightTwo.intensity, 0.0f, Time.deltaTime*1000.0f);
        LightOne.intensity = Mathf.Lerp(LightOne.intensity, 0.0f, Time.deltaTime*1000.0f);

        if (SoundController.Get().MidiBass)
        {
            LightTwo.intensity = 1e+14f;
            LightOne.intensity = 1e+14f;
        }

        float lerp = _Timer*0.5f;

        Cam.position = Vector3.Lerp(Cameras.GetChild(_CamID).position, Cameras.GetChild(_CamID).GetChild(0).position, lerp);
        Cam.rotation = Quaternion.Lerp(Cameras.GetChild(_CamID).rotation, Cameras.GetChild(_CamID).GetChild(0).rotation, lerp);

        if (Music.time>=174.5f)
        {
            gameObject.SetActive(false);
            NextScene.SetActive(true);
        }
    }
}
