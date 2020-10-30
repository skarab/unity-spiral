using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deform : MonoBehaviour
{
    public AudioSource Music;
    public RawImage Render;
    public Texture2D[] ScreenDeforms;
    public Texture2D[] SquareDeforms;
    public Texture2D[] Glitches;

    private float _Timer = 0.0f;
    private float _ChangeTimer = 0.0f;
    private int _DeformID = 0;

    private Matrix4x4 _Mat = new Matrix4x4();

    void Start()
    {
        Render.material.SetFloat("_DeformSquare", 0.0f);
    }

    void Update()
    {
        _Timer += Time.deltaTime;
        _ChangeTimer += Time.deltaTime;

        if (SoundController.Get().Bass>=1.0f && _ChangeTimer>0.8f)
        {
            _ChangeTimer = 0.0f;
            _Timer = 0.0f;
            _DeformID = (_DeformID+1)%(ScreenDeforms.Length*2);
        }

        Render.material.SetFloat("_DeformInvert", _DeformID>=ScreenDeforms.Length?1.0f:0.0f);
        Render.material.SetTexture("_DeformTex", ScreenDeforms[_DeformID%ScreenDeforms.Length]);

        float lerp = _Timer*1.6f;
        _Mat.SetTRS(Vector3.zero, Quaternion.Euler(0.0f, 0.0f, Mathf.Lerp(0.0f, 180.0f, lerp)), Vector3.one);
        Render.material.SetMatrix("_DeformRoll", _Mat);

        int id = (int)(Music.time*20.0f)%(SquareDeforms.Length*2);
        Render.material.SetFloat("_DeformSquareInvert", id>=SquareDeforms.Length?1.0f:0.0f);
        Render.material.SetTexture("_DeformSquareTex", SquareDeforms[id%SquareDeforms.Length]);

        Render.material.SetTexture("_Glitch", Glitches[(int)(Music.time*20.0f)%(Glitches.Length)]);
    }
}
