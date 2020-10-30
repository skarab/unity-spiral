using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class SceneFlow : MonoBehaviour
{
    public Transform Girl;
    public Transform Cam;
    public AudioSource Music;
    public float Speed;
    public Transform[] Feet;
    public Transform[] Steps;
    public VisualEffect ParticlesDots;
    public VisualEffect ParticlesLines;
    public GameObject NextScene;
    public Material Channel6;
    public Material Channel8;
    public RawImage Render;

    private float _Timer = 0.0f;

    private Color[] _Colors =
    {
        new Color(1.0f, 0.5f, 0.0f),
        new Color(1.0f, 1.0f, 0.0f),
        new Color(0.4f, 1.0f, 0.4f),
        new Color(0.1f, 0.4f, 1.0f)
    };
    private int _ColorID = 0;

    void Start()
    {        
        Render.material.SetFloat("_DeformSquare", 1.0f);

        for (int i=0 ; i<Steps.Length ; ++i)
        {
            for (int j=0 ; j<Steps[i].childCount ; ++j)
            {
                Steps[i].GetChild(j).GetChild(0).GetComponent<Renderer>().material.SetColor("_EmissiveColor", Color.black);
            }
        }

        ParticlesDots.SetFloat("Rate", 0.0f);
        ParticlesLines.SetFloat("Rate", 0.0f);

        Channel6.SetColor("_EmissiveColor", Color.black);
        Channel8.SetColor("_EmissiveColor", Color.black);
    }

    void Update()
    {
        _Timer += Time.deltaTime;

        Girl.position = new Vector3(Girl.position.x, Girl.position.y, Mathf.MoveTowards(Girl.position.z, 254.1f, Time.deltaTime*Speed));

        for (int i=0 ; i<Steps.Length ; ++i)
        {
            for (int j=0 ; j<Steps[i].childCount ; ++j)
            {
                Renderer renderer = Steps[i].GetChild(j).GetChild(0).GetComponent<Renderer>();
                renderer.material.SetColor("_EmissiveColor", Color.Lerp(renderer.material.GetColor("_EmissiveColor"), Color.black, Time.deltaTime));
            }
        }

        for (int i=0 ; i<Feet.Length ; ++i)
        {
            Ray ray = new Ray(Feet[i].position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.distance<1.0f)
            {
                hit.collider.GetComponent<Renderer>().material.SetColor("_EmissiveColor", _Colors[_ColorID]*5000.0f);
                _ColorID = (_ColorID+1)%_Colors.Length;

                if (hit.collider.transform.parent.name=="end")
                {
                    ParticlesDots.SetFloat("Rate", 10000.0f);
                    ParticlesLines.SetFloat("Rate", 1000.0f);
                }
            }
        }

        Channel6.SetColor("_EmissiveColor", Color.Lerp(Channel6.GetColor("_EmissiveColor"), Color.black, Time.deltaTime*10.0f));
        Channel8.SetColor("_EmissiveColor", Color.Lerp(Channel8.GetColor("_EmissiveColor"), Color.black, Time.deltaTime*10.0f));
         
        if (SoundController.Get().SampleChannel(6))
            Channel6.SetColor("_EmissiveColor", new Color(1.0f, 1.0f, 1.0f)*4000.0f);
        if (SoundController.Get().SampleChannel(8))
            Channel8.SetColor("_EmissiveColor", new Color(1.0f, 0.2f, 0.0f)*4000.0f);
        
        if (SoundController.Get().SampleChannel(8))
            Render.material.SetFloat("_Scanline", 0.5f);

        if (_Timer>=25.0f)
        {
            gameObject.SetActive(false);
            NextScene.SetActive(true);
            SoundController.Get().Pixelate();
        }
    }
}
