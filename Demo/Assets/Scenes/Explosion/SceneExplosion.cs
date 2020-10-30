using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneExplosion : MonoBehaviour
{
    public RawImage RenderShader;
    public Material RenderMat;
    public Transform Cam;
    public Transform Cameras;
    public AudioSource Music;
    public Transform Position;
    public GameObject RedLight;
    public GameObject NextScene;

    private float _Timer = 0.0f;
    private int _CamID = 0;

    void Start()
    {
        RenderShader.enabled = true;
        RenderMat.SetFloat("_Deform", 1.0f);
        //RenderUI.Get().SetColor(new Color(0.0f, 0.0f, 0.0f, 0.7f));
    }

    void Update()
    {
        RenderShader.material.SetFloat("_MyTime", Time.time*15.0f+SoundController.Get().Bass*2.0f);

        _Timer += Time.deltaTime;
        
        if (SoundController.Get().MidiBass)
        {
            _Timer = 0.0f;
            _CamID = (_CamID+1)%Cameras.childCount;
        }

        float lerp = _Timer*1.0f;

        Cam.position = Vector3.Lerp(Cameras.GetChild(_CamID).position, Cameras.GetChild(_CamID).GetChild(0).position, lerp);
        Cam.rotation = Quaternion.Lerp(Cameras.GetChild(_CamID).rotation, Cameras.GetChild(_CamID).GetChild(0).rotation, lerp);
    
        RenderShader.material.SetVector("_CamPos", Cam.position-Position.position);
        RenderShader.material.SetVector("_CamDir", Cam.forward);

        RedLight.SetActive(((int)(_Timer*12.0f))%2==0);

        if (Music.time>=102.0f)
        {
            RenderShader.enabled = false;
            gameObject.SetActive(false);
            NextScene.SetActive(true);
            SoundController.Get().Pixelate();
        }
    }
}
