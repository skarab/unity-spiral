using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rebels : MonoBehaviour
{
    public RawImage Logo;
    public RawImage Things;
    public Material Render;
    public AudioSource Music;

    void Start()
    {
        float border = UnityEngine.Screen.width*0.03f;
        RectTransform p = transform as RectTransform;

        Logo.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 429.0f*UnityEngine.Screen.width/1920.0f);
        Logo.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 178.0f*UnityEngine.Screen.width/1920.0f);
        Logo.rectTransform.anchoredPosition = new Vector2(border, p.rect.height-border-Logo.rectTransform.rect.height);
        Logo.material.SetFloat("_Fade", 0.0f);
        
        Things.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 778.0f*UnityEngine.Screen.width/1920.0f);
        Things.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 178.0f*UnityEngine.Screen.width/1920.0f);
        Things.rectTransform.anchoredPosition = new Vector2(p.rect.width-border-Things.rectTransform.rect.width, p.rect.height-border-Things.rectTransform.rect.height);
        Things.material.SetFloat("_Fade", 0.0f);
    }

    void Update()
    {
        float alpha = (Music.time<218.0f || Music.time>=228.0f)?0.0f:1.0f;
       
        Logo.material.SetFloat("_Scanline", Render.GetFloat("_Scanline"));
        Logo.material.SetFloat("_Height", Logo.rectTransform.rect.height);
        Logo.material.SetFloat("_Fade", Mathf.Lerp(Logo.material.GetFloat("_Fade"), alpha, Time.deltaTime*0.5f));
        
        Things.material.SetFloat("_Scanline", Render.GetFloat("_Scanline"));
        Things.material.SetFloat("_Height", Things.rectTransform.rect.height);
        Things.material.SetFloat("_Fade", Mathf.Lerp(Things.material.GetFloat("_Fade"), alpha, Time.deltaTime*0.5f));
    }
}
