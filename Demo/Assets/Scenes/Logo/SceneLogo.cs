using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneLogo : MonoBehaviour
{
    public Transform Logo;
    public AudioSource Music;
    public GameObject NextScene;
    public RawImage Render;

    void Start()
    {        
    }

    void Update()
    {
        Logo.Rotate(Vector3.forward, Time.deltaTime*300.0f);
    
        if (Music.time>=160.0f)
        {
            gameObject.SetActive(false);
            NextScene.SetActive(true);
        }

        if (SoundController.Get().SampleChannel(6))
            Render.material.SetFloat("_Scanline", 1.0f);
    }
}
