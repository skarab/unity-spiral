using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class SceneSpace : MonoBehaviour
{
    private const float _ScannerZMin = -170.0f;
    private const float _ScannerZMax = -34.0f;

    public RawImage Render;
    public Transform Ship;
    public AudioSource Music;
    public Transform Scanner;
    public Renderer EmitterBase;
    public Renderer Emitter;
    public VisualEffect Particles;  
    public Transform Cam;    
    public Transform CamLogo;
    public Transform CamEnd;
    public Transform CamRandoms;
    public GameObject NextScene;
    public GameObject NextScene2;
    public Material RenderMat;
    public Volume PPVolume;
    public Transform Asteroids;

    private Dictionary<Transform, Vector3> _Positions;
    private Dictionary<Transform, Quaternion> _Rotations;

    private float _Timer = 0.0f;
    private int _CamID = 0;

    private bool _PrintedCredit = false;

    void Awake()
    {
        Render.material.SetFloat("FadeOut", 0.0f);
        Render.material.SetFloat("_GlitchStrength", 0.0f);
        Render.material.SetFloat("DeformSquare", 0.0f);
        
        _Positions = new Dictionary<Transform, Vector3>();
        _Rotations = new Dictionary<Transform, Quaternion>();

        MapTransform(CamLogo);
        MapTransform(CamEnd);

        for (int i=0 ; i<CamRandoms.childCount ; ++i)
            MapTransform(CamRandoms.GetChild(i));

        Ship.transform.position = new Vector3(0.0f, 0.0f, 100.0f);

        for (int i=0 ; i<Asteroids.childCount ; ++i)
            Asteroids.GetChild(i).GetComponent<Renderer>().material.SetColor("_EmissiveColor", new Color(248.0f/255.0f, 88.0f/255.0f, 88.0f/255.0f)*(float)(Random.value*6e+07));
    }

    void OnStart()
    {
        Render.material.SetFloat("_FadeOut", 1.0f);
        
        EmitterBase.sharedMaterials[0].SetFloat("_EmissiveExposureWeight", 1.0f);
        Emitter.sharedMaterials[0].SetFloat("_EmissiveExposureWeight", 1.0f);
        Particles.SetFloat("Min", 0.0f);
        Particles.SetFloat("Max", 0.0f);

        if (Music.time>=73.5f)
        {
            Render.material.SetFloat("_FadeOut", 0.0f);
            Ship.transform.position = new Vector3(0.0f, 0.0f, -1200.0f);            
        }

        _Timer = 0.0f;
        _CamID = 0;

        RenderUI.Get().SetColor(new Color(1.0f, 0.6f, 0.0f, 0.7f));    
    }

    private void OnEnable()
    {
        OnStart();
    }

    private void MapTransform(Transform trs)
    {
        _Positions[trs] = trs.position;
        _Rotations[trs] = trs.rotation;
        _Positions[trs.GetChild(0)] = trs.GetChild(0).position;
        _Rotations[trs.GetChild(0)] = trs.GetChild(0).rotation;
    }

    void Update()
    {
        if (!Music.isPlaying)
            return;

        Render.material.SetFloat("_FadeOut", Mathf.MoveTowards(Render.material.GetFloat("_FadeOut"), 0.0f, Time.deltaTime*0.1f));
    
        Ship.transform.position = new Vector3(0.0f, 0.0f, Ship.transform.position.z-Time.deltaTime*16.0f);


        float speed = Mathf.Clamp((Music.time-3.5f)*0.15f, 0.0f, 4.0f);
        float p = Mathf.Repeat(Time.time*speed, 1.0f);
        float s = 0.15f*(p<0.01f?p*100.0f:p>0.99f?1.0f-(p-0.99f)*100.0f:1.0f);
        float a = _ScannerZMin+(_ScannerZMax-_ScannerZMin)*p;
        s *= 1.0f-Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad*a))*0.3f;

        if (Music.time<3.5f) s = 0.0f;

        if (Music.time>3.0f)
        {
            EmitterBase.sharedMaterials[0].SetFloat("_EmissiveExposureWeight", 0.993f);
        }
        if (Music.time>3.25f)
        {
            Emitter.sharedMaterials[0].SetFloat("_EmissiveExposureWeight", 0.92f);
        }

        Scanner.localRotation = Quaternion.Euler(0.0f, 0.0f, a);
        Scanner.localScale = new Vector3(s, s, s);

        float pp_size = Mathf.Clamp01(speed*0.23f);
        float pp_min = p-pp_size/2.0f;
        float pp_max = p+pp_size/2.0f;
        Particles.SetFloat("Min", pp_min);
        Particles.SetFloat("Max", pp_max);

        Render.material.SetFloat("_GlitchStrength", 0.0f);
        Render.material.SetFloat("DeformSquare", 0.0f);
        
        DepthOfField dof;
        PPVolume.sharedProfile.TryGet<DepthOfField>(out dof);  
       
        if (Music.time<14.5f)
        {
            Cam.localPosition = Vector3.Lerp(_Positions[CamLogo], _Positions[CamLogo.GetChild(0)], Music.time/14.5f);
            Cam.localRotation = Quaternion.Lerp(_Rotations[CamLogo], _Rotations[CamLogo.GetChild(0)], Music.time/14.5f);
            dof.active = true;
            dof.farFocusStart.value = Mathf.Lerp(10.0f, 1000.0f, Music.time/20.0f);
            dof.farFocusEnd.value = Mathf.Lerp(50.0f, 10000.0f, Music.time/20.0f);
        }
        else if (Music.time>=41.5f && Music.time<73.5f)
        {
            Cam.localPosition = Vector3.Lerp(_Positions[CamEnd], _Positions[CamEnd.GetChild(0)], (Music.time-41.5f)/2.0f);
            Cam.localRotation = Quaternion.Lerp(_Rotations[CamEnd], _Rotations[CamEnd.GetChild(0)], (Music.time-41.5f)/2.0f);

            Render.material.SetFloat("_GlitchStrength", 1.0f);
            Render.material.SetFloat("DeformSquare", 1.0f);        
        }
        else
        {
            _Timer += Time.deltaTime;
            
            if (SoundController.Get().MidiBassTwo && Music.time<41.5f)
            {
                _Timer = 0.0f;
                _CamID = (_CamID+1)%CamRandoms.childCount;
            }

            if (SoundController.Get().MidiBass && Music.time>=73.5f)
            {
                _Timer = 0.0f;
                _CamID = (_CamID+1)%CamRandoms.childCount;
            }

            float lerp = _Timer*1.3f;

            if (Music.time<41.5f)
                lerp = _Timer*0.95f;

            Cam.localPosition = Vector3.Lerp(_Positions[CamRandoms.GetChild(_CamID)], _Positions[CamRandoms.GetChild(_CamID).GetChild(0)], lerp);
            Cam.localRotation = Quaternion.Lerp(_Rotations[CamRandoms.GetChild(_CamID)], _Rotations[CamRandoms.GetChild(_CamID).GetChild(0)], lerp);
        }

        RenderMat.SetFloat("_Deform", Music.time>=73.5f?0.5f:(Music.time>=29.3f && Music.time<41.5f)?1.0f:0.0f);
    
        if (Music.time>=73.5f)
        {
            if (Music.time>=80.0f && !_PrintedCredit)
            {
                _PrintedCredit = true;
                RenderUI.Get().EnableTV(true);
                RenderUI.Get().Print("adapt\naenima\nand\nblasphemy\nbrainstorm\ncocoon\ncondense\ncritical mass\ndeadliners\ndesire\ndigital murder\nekspert\nfairlight\nfarbrausch\nflush\nfocus design\ngenesis project\nk2\nk-storm\nkiki-prods\nknights\nlego\nlemon.\nlimp ninja\nlnx\nlogicoma\nlos tacos\npolka brothers\npopsy team\nresistance\nskarla\nstill\nsuburban\nsyn[rj]\ntbl\nthe lost souls\ntitan\ntraction\ntrsi\n\n");
            }

            if (Music.time>=87.0f)
            {
                gameObject.SetActive(false);
                NextScene2.SetActive(true);
                SoundController.Get().Pixelate();
            }
        }
        else if (Music.time>=43.7f)
        {
            gameObject.SetActive(false);
            NextScene.SetActive(true);
            SoundController.Get().Pixelate();
        }
    }
}
