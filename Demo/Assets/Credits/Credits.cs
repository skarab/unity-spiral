using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Credits : MonoBehaviour
{
    private const float _Length = 12.0f;

    public AudioSource Music;
    public RectTransform Duke;
    public RectTransform Unreal;
    public RectTransform Klos;
    public RectTransform Teo;
    public RectTransform Skarab;
    public Material Render;
    public Texture2D[] SquareDeforms;

    void Start()
    {
    }

    
    void Update()
    {
        MakeCredit(Unreal, 0.0f, 0.0f, 45.0f);
        MakeCredit(Duke, 1.0f, 1.0f, 90.0f);
        MakeCredit(Skarab, 0.0f, 0.0f, 135.0f);
        MakeCredit(Teo, 0.0f, 1.0f, 163.0f);
        MakeCredit(Klos, 1.0f, 1.0f, 182.0f);
    }

    private void MakeCredit(RectTransform trs, float x, float y, float time_start)
    {
        float length = _Length;
        if (trs==Duke || trs==Klos)
            length = 7.0f;

        float fade_time = length/2.0f;
        float time_end = time_start+length;

        if (Music.time>=time_start && Music.time<time_end)
        {
            trs.gameObject.SetActive(true);

            RectTransform screen = transform as RectTransform;
            float border = screen.rect.width*0.02f;

            RawImage image = trs.GetComponent<RawImage>();
            float w = image.texture.width*UnityEngine.Screen.width/1920.0f;
            float h = image.texture.height*UnityEngine.Screen.width/1920.0f;
            
            trs.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
            trs.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
            
            trs.anchoredPosition = new Vector2(Mathf.Lerp(border, screen.rect.width-border-w, x), Mathf.Lerp(border, screen.rect.height-border-h, y));
        
            float fade = 1.0f;
            if (Music.time-time_start<fade_time) fade = (Music.time-time_start)/fade_time;
            else if (Music.time>=time_end-fade_time) fade = 1.0f-(Music.time-(time_end-fade_time))/fade_time;

            image.material.SetFloat("_Fade", fade);

            if (trs==Klos) image.material.SetFloat("_White", (Music.time-time_start)/(time_end-time_start));
            else image.material.SetFloat("_White", 0.0f);

            image.material.SetFloat("_Scanline", Render.GetFloat("_Scanline"));
            image.material.SetFloat("_Height", h);

            int id = (int)(Music.time*20.0f)%(SquareDeforms.Length*2);
            image.material.SetFloat("_DeformSquareInvert", id>=SquareDeforms.Length?1.0f:0.0f);
            image.material.SetTexture("_DeformSquareTex", SquareDeforms[id%SquareDeforms.Length]);
        }
        else
        {
            trs.gameObject.SetActive(false);
        }
    }
}
