using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneShader : MonoBehaviour
{
    public AudioSource Music;
    public GameObject NextScene;

    void Start()
    {
    }

    void Update()
    {
        if (Music.time>=189.0f)
        {
            gameObject.SetActive(false);
            NextScene.SetActive(true);
            SoundController.Get().Pixelate();
        }
    }
}
