using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class RenderCamera : MonoBehaviour
{
    public RawImage Target;
    private RenderTexture Tex;

    void Awake()
    {
        Tex = new RenderTexture((int)Target.rectTransform.rect.width, (int)Target.rectTransform.rect.height, 24, RenderTextureFormat.ARGB32);
        Tex.Create();
        GetComponent<Camera>().targetTexture = Tex;
        Target.material.SetTexture("_Tex", Tex);    
    }

    void OnEnable()
    {
        Awake();
    }

}
