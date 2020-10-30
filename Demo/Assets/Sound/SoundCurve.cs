using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundCurve : MonoBehaviour
{
    public Texture2D IconOn;
    public Texture2D IconOff;
    public RawImage Bass;
    public TMPro.TextMeshProUGUI Title;
    public RectTransform Curve;
    public GameObject Line;
    public AudioSource Music;

    private Color _ColorOn = new Color(0.0f, 1.0f, 1.0f, 0.8f);
    private Color _ColorOff = new Color(0.0f, 1.0f, 1.0f, 0.2f);
    private const float _ColorSpeed = 2.0f;

    private const int _SpectrumSize = 64;
    private const float _SpectrumScale = 400.0f;
    private float[] _Spectrum = new float[_SpectrumSize];
    
    void Start()
    {
        float border = UnityEngine.Screen.width*0.01f;

        RectTransform trs = transform as RectTransform;
        float w = 440.0f*UnityEngine.Screen.width/1920.0f;
        float h = 120.0f*UnityEngine.Screen.width/1920.0f;
        trs.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        trs.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
        trs.anchoredPosition = new Vector2(border, border);

        Bass.color = _ColorOff;

        Title.fontSize = 33.72f*UnityEngine.Screen.width/1920.0f;

        for (int i=0 ; i<_SpectrumSize-1 ; ++i)
            Object.Instantiate<GameObject>(Line, Curve);
    }
        
    void Update()
    {
        bool enabled = Music.time>=189.0f && Music.time<=216.5f;

        if (enabled)
        {
            Bass.texture = SoundController.Get().MidiBass?IconOn:IconOff;
            Bass.color = Color.Lerp(Bass.color, _ColorOff, Time.deltaTime*_ColorSpeed);

            if (SoundController.Get().MidiBass)
                Bass.color = _ColorOn;

            AudioListener.GetSpectrumData(_Spectrum, 0, FFTWindow.Rectangular);
        
            for (int i=0 ; i<Curve.childCount ; ++i)
            {
                Vector2 p1 = new Vector2(i*Curve.rect.width/Curve.childCount, Mathf.Min(_Spectrum[i]*_SpectrumScale, 1.0f)*Curve.rect.height);
                Vector2 p2 = new Vector2((i+1)*Curve.rect.width/Curve.childCount, Mathf.Min(_Spectrum[i+1]*_SpectrumScale, 1.0f)*Curve.rect.height);

                RectTransform trs = Curve.GetChild(i) as RectTransform;
                trs.eulerAngles = new Vector3(0.0f, 0.0f, -Vector2.SignedAngle((p2-p1).normalized, Vector2.right));
                trs.anchoredPosition = p1;
                trs.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Vector2.Distance(p1, p2));
            }
        }

        float border = UnityEngine.Screen.width*0.01f;
        (transform as RectTransform).anchoredPosition = new Vector2(border-(enabled?0.0f:UnityEngine.Screen.width), border);
    }
}
