using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class SceneIsland : MonoBehaviour
{
    private const float _GenTimer = 0.1f;

    public Transform[] CubeEmitters;
    public GameObject CubePrefab;
    public Transform Cubes;
    public VisualEffect ParticlesDeath;
    public Transform Cam;
    public Transform Cameras;
    public AudioSource Music;
    public Volume PPVolume;
    public VolumeProfile Sky;
    public VisualEffect ParticlesClouds;
    public GameObject NextScene;
    public GameObject Asteroids;
    public GameObject Planets;
    public RawImage Render;
    
    private float _Timer = 0.0f;
    private int _CamID = 0;

    private float _Gen = 0.0f;

    private float _ExTimer = 0.0f;
    private bool _Pixelate = false;

    void Start()
    {
        Render.material.SetFloat("_GlitchStrength", 0.0f);
        Render.material.SetFloat("DeformSquare", 0.0f);
        
        CubeEmitters[0].GetComponent<VisualEffect>().SetVector3("color", new Vector3(0.0f, 0.0f, 0.0f));
        CubeEmitters[1].GetComponent<VisualEffect>().SetVector3("color", new Vector3(0.0f, 0.0f, 0.0f));
        Asteroids.SetActive(true);
        Planets.SetActive(true);

        for (int i=0 ; i<Asteroids.transform.childCount ; ++i)
            Asteroids.transform.GetChild(i).GetComponent<Renderer>().material.SetColor("_EmissiveColor", new Color(248.0f/255.0f, 88.0f/255.0f, 88.0f/255.0f)*(float)(Random.value*6e+07));
    }

    void Update()
    {
        ParticlesClouds.SetInt("Rate", (int)(SoundController.Get().Bass*10000));

        _ExTimer += Time.deltaTime;

        HDRISky sky;
        Sky.TryGet<HDRISky>(out sky);
        sky.exposure.value = Mathf.Lerp(16.45f, 12.05f, _ExTimer*0.05f);
        Exposure ex;
        PPVolume.sharedProfile.TryGet<Exposure>(out ex);  
        ex.fixedExposure.value = Mathf.Lerp(20.8f, 23.9f, _ExTimer*0.05f);

        if (Music.time>=58.3f)
        {
            CubeEmitters[0].GetComponent<VisualEffect>().SetVector3("color", new Vector3(200.0f, 20.0f, 0.0f));
            CubeEmitters[1].GetComponent<VisualEffect>().SetVector3("color", new Vector3(50.0f, 100.0f, 200.0f));
        
            _Gen += Time.deltaTime;
            if (_Gen>=_GenTimer)
            {
                _Gen = 0.0f;

                int i = (int)(Random.value*(CubeEmitters.Length));
                GameObject cube = GameObject.Instantiate<GameObject>(CubePrefab, CubeEmitters[i].position, Quaternion.identity, Cubes);
                cube.GetComponent<Cube>().ParticlesDeath = ParticlesDeath;
                cube.GetComponent<Renderer>().material.SetColor("_EmissiveColor", (i==0?new Color(1.0f, 0.2f, 0.0f):new Color(0.1f, 0.5f, 1.0f))*400.0f);
            }
        }

        _Timer += Time.deltaTime;
        
        if (SoundController.Get().MidiBassTwo && Music.time<58.5f)
        {
            _Timer = 0.0f;
            _CamID = (_CamID+1)%(Cameras.childCount-1);
        }

        if (SoundController.Get().MidiBass && Music.time>63.0f)
        {
            _Timer = 0.0f;
            _CamID = (_CamID+1)%(Cameras.childCount-1);
        }

        float lerp_speed = 1.3f;
        if (Music.time<58.5f)
            lerp_speed = 0.95f;

        if (Music.time>=58.0f && Music.time<=63.0f)
        {
            if (!_Pixelate)
            {
                SoundController.Get().Pixelate();
                _Pixelate = true;
            }

            //Asteroids.SetActive(false);
            //Planets.SetActive(false);

            lerp_speed = 0.18f;
            if (_CamID!=Cameras.childCount-1)
            {
                _Timer = 0.0f;
                _CamID = Cameras.childCount-1;
            }
        }

        if (Music.time>63.0f && _Pixelate)
        {
            SoundController.Get().Pixelate();
            _Pixelate = false;
        }

        float lerp = _Timer*lerp_speed;

        Cam.position = Vector3.Lerp(Cameras.GetChild(_CamID).position, Cameras.GetChild(_CamID).GetChild(0).position, lerp);
        Cam.rotation = Quaternion.Lerp(Cameras.GetChild(_CamID).rotation, Cameras.GetChild(_CamID).GetChild(0).rotation, lerp);

        if (Music.time>=73.5f)
        {
            gameObject.SetActive(false);
            NextScene.SetActive(true);
            SoundController.Get().Pixelate();
        }
    }
}
