using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBabies : MonoBehaviour
{
    private const float _FlowTime = 0.1f;
    private const float _DoorSpeed = 2.0f;

    public GameObject Baby;
    public Transform Babies;
    public Transform DoorLeft;
    public Transform DoorRight;
    public AudioSource Music;
    public GameObject NextScene;

    private float _Timer = 0.0f;
    private float _DoorTimer = 0.0f;

    void Start()
    {
        DoorLeft.rotation = Quaternion.identity;
        DoorRight.rotation = Quaternion.identity;
    }

    void Update()
    {
        _DoorTimer += Time.deltaTime;

        if (_DoorTimer>=0.5f)
        {
            DoorLeft.rotation = Quaternion.Lerp(DoorLeft.rotation, Quaternion.Euler(new Vector3(0.0f, 130.0f, 0.0f)), Time.deltaTime*_DoorSpeed);
            DoorRight.rotation = Quaternion.Lerp(DoorRight.rotation, Quaternion.Euler(new Vector3(0.0f, -130.0f, 0.0f)), Time.deltaTime*_DoorSpeed);
        }

        if (_DoorTimer>=1.5f)
        {
            _Timer += Time.deltaTime;

            if (_Timer>=_FlowTime)
            {
                _Timer = 0.0f;

                GameObject.Instantiate<GameObject>(Baby, Vector3.forward*-2.0f, Quaternion.identity, Babies);
            }
        }

        if (Music.time>=232.0f)
        {
            gameObject.SetActive(false);
            NextScene.SetActive(true);
            SoundController.Get().Pixelate();
        }
    }
}
