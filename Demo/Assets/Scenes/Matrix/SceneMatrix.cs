using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMatrix : MonoBehaviour
{
    public GameObject NextScene;

    private float _Timer = 0.0f;

    void Start()
    {
    }

    void Update()
    {
        _Timer += Time.deltaTime;
    
        if (_Timer>=27.5f)
        {
            gameObject.SetActive(false);
            NextScene.SetActive(true);
            SoundController.Get().Pixelate();
        }
    }
}
