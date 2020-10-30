using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLandscape : MonoBehaviour
{
    private const float _SpawnRate = 3.0f;
    private const float _Speed = 0.25f;
    private const float _CubeSpeed = 0.1f;

    public Renderer[] Spheres;
    public Transform[] Emitters;
    public Transform Target;
    public GameObject Baby;
    public Transform Nodes;
    public Material Platform;
    public Transform Cubes;
    public GameObject Cube;
    public Material RenderMat;

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

    private Color[] _CubeColors =
    {
        new Color(1.0f, 0.5f, 0.0f),
        new Color(0.0f, 0.5f, 1.0f),
    };

    private float _SpawnTimer = 0.0f;

    void Start()
    {
        for (int i=0 ; i<Spheres.Length ; ++i)
            Spheres[i].material.SetColor("_EmissiveColor", _SphereColors[i]*0.0f);

        Platform.SetColor("_EmissiveColor", Color.black);

        RenderUI.Get().PrintScroller("Spiral\n\nRebels, 2020\n\nskarab (code, modeling, direction)\nteo (music, direction)\nunreal (overlays, graphics)\nspolsh aka klos (crystal drops shader)\nduke (volumetric explosion shader)\n\nfirst shown at function 2020\npresented in 2.20:1\nnobile open source font\nneoqueto (kleenblades font)\ngregor adams (pbio font)\nazdm (spaceship)\ndactilardesign (cabin texture)\nkevin iglesias (motion pack)\n\n\nadapt\naenima\nand\nblasphemy\nbrainstorm\ncocoon\ncondense\ncritical mass\ndeadliners\ndesire\ndigital murder\nekspert\nfairlight\nfarbrausch\nflush\nfocus design\ngenesis project\nk2\nk-storm\nkiki-prods\nknights\nlego\nlemon.\nlimp ninja\nlnx\nlogicoma\nlos tacos\npolka brothers\npopsy team\nresistance\nskarla\nstill\nsuburban\nsyn[rj]\ntbl\nthe lost souls\ntitan\ntraction\ntrsi\n\n\n\n\"There is no bass.\", Ozan\n\n\nsome tools :\nunity 2019.4.8f1\nblender 2.83.4\ngimp 2.10.20\nmakehuman 1.2.0 beta\n...\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nCopyright © Rebels, 2020\n");
    
        RenderMat.SetFloat("_Deform", 1.0f);
    }
    
    void Update()
    {
        for (int i=0 ; i<Spheres.Length ; ++i)
        {
            Spheres[i].material.SetColor("_EmissiveColor", Color.Lerp(Spheres[i].material.GetColor("_EmissiveColor"), Color.black, Time.deltaTime*40.0f));

            if (SoundController.Get().Sample(i))
                Spheres[i].material.SetColor("_EmissiveColor", _SphereColors[i]*20000.0f);
        }

        _SpawnTimer += Time.deltaTime;

        if (_SpawnTimer>=1.0f/_SpawnRate)
        {
            _SpawnTimer = 0.0f;

            GameObject.Instantiate<GameObject>(Baby, Emitters[(int)(Random.value*(Emitters.Length-1))].position, Quaternion.identity, Nodes);
        }

        Platform.SetColor("_EmissiveColor", Color.Lerp(Platform.GetColor("_EmissiveColor"), Color.black, Time.deltaTime*20.0f));

        for (int i=0 ; i<Cubes.childCount ; ++i)
        {
            Transform cube = Cubes.GetChild(i);
            cube.transform.position += new Vector3(Random.value-0.5f, 3.0f, Random.value-0.5f).normalized*_CubeSpeed;
            cube.transform.Rotate(new Vector3(Random.value-0.5f, 3.0f, Random.value-0.5f).normalized, Time.deltaTime*60.0f);

            if (cube.transform.position.y>=10.0f)
            {
                DestroyImmediate(cube.gameObject);
                --i;
            }
        }

        for (int i=0 ; i<Nodes.childCount ; ++i)
        {
            Transform node = Nodes.GetChild(i);
            Vector3 old_position = node.position;
            node.position = Vector3.MoveTowards(node.position, Target.position, Time.deltaTime*_Speed);
            node.rotation = Quaternion.LookRotation((node.position-old_position).normalized, Vector3.up);

            if (node.position==Target.position)
            {
                DestroyImmediate(node.gameObject);
                --i;

                Platform.SetColor("_EmissiveColor", Color.white*2000.0f);

                GameObject cube = GameObject.Instantiate<GameObject>(Cube, Target.position, Quaternion.identity, Cubes);
                cube.GetComponent<Renderer>().material.SetColor("_EmissiveColor", _CubeColors[(int)(Random.value*(_CubeColors.Length-1))]*50.0f);
            }
        }
    }
}
