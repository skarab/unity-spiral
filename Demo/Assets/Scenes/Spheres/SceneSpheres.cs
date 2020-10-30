using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class SceneSpheres : MonoBehaviour
{
    public Renderer[] Spheres;
    public Transform Cam;
    public Transform Cameras;
    public AudioSource Music;
    public VisualEffect Particles;
    public GameObject NextScene;
    public RawImage Render;

    private Color[] _SphereColors =
    {
        new Color(1.0f, 1.0f, 1.0f),
        new Color(1.0f, 0.0f, 0.0f),
        new Color(0.0f, 1.0f, 0.0f),
        new Color(0.0f, 0.0f, 1.0f),
        new Color(1.0f, 0.5f, 0.0f),
        new Color(0.0f, 1.0f, 0.5f),
        new Color(0.5f, 1.0f, 0.0f),
        new Color(0.0f, 0.5f, 1.0f),
        new Color(0.5f, 0.0f, 1.0f),
        new Color(1.0f, 0.0f, 0.5f),
        new Color(1.0f, 1.0f, 0.0f),
        new Color(0.0f, 1.0f, 1.0f)
    };

    private float _Timer = 0.0f;
    private int _CamID = 0;

    void Start()
    {
        Render.material.SetFloat("_GlitchStrength", 1.0f);

        for (int i=0 ; i<Spheres.Length ; ++i)
            Spheres[i].material.SetColor("_EmissiveColor", _SphereColors[i]*0.0f);

        Particles.SetFloat("Strength", 0.0f);
    }

    
    void Update()
    {
        Particles.SetFloat("Strength", Mathf.Lerp(Particles.GetFloat("Strength"), 1.0f, Time.deltaTime*0.5f));
    
        for (int i=0 ; i<Spheres.Length ; ++i)
        {
            Spheres[i].material.SetColor("_EmissiveColor", Color.Lerp(Spheres[i].material.GetColor("_EmissiveColor"), Color.black, Time.deltaTime*40.0f));

            if (SoundController.Get().Sample(i))
                Spheres[i].material.SetColor("_EmissiveColor", _SphereColors[i]*20000.0f);
        }
        
        _Timer += Time.deltaTime;
        
        if (SoundController.Get().MidiBass && (Music.time<129.5f || Music.time>158.0f))
        {
            _Timer = 0.0f;
            _CamID = (_CamID+1)%Cameras.childCount;
        }

        if (Music.time>=129.5f && Music.time<=158.0f && _CamID!=Cameras.childCount-1)
        {
            _Timer = 0.0f;
            _CamID = Cameras.childCount-1;
        }

        float lerp = _Timer*1.0f;

        Cam.position = Vector3.Lerp(Cameras.GetChild(_CamID).position, Cameras.GetChild(_CamID).GetChild(0).position, lerp);
        Cam.rotation = Quaternion.Lerp(Cameras.GetChild(_CamID).rotation, Cameras.GetChild(_CamID).GetChild(0).rotation, lerp);

        if (Music.time>=130.0f)
        {
            gameObject.SetActive(false);
            NextScene.SetActive(true);
            SoundController.Get().Pixelate();
        }
    }
}
