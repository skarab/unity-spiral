using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class SoundBar : MonoBehaviour
{
    public Texture2D IconOn;
    public Texture2D IconOff;
    public RawImage[] Channels;
    public RectTransform Bass;
    public RectTransform Treble;
    public RectTransform[] Corners;
    public AudioSource Music;

    private Color _ColorOn = new Color(1.0f, 1.0f, 1.0f, 0.8f);
    private Color _ColorOff = new Color(1.0f, 1.0f, 1.0f, 0.2f);
    private const float _ColorSpeed = 2.0f;

    void Start()
    {
        float w = (transform.parent as RectTransform).rect.width*0.15f;
        float h = w/12.0f;
        RectTransform trs = transform as RectTransform;
        trs.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        trs.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);

        for (int i=0 ; i<Channels.Length ; ++i)
        {
            Channels[i].color = _ColorOff;
        }

        for (int i=0 ; i<Corners.Length ; ++i)
        {
            Corners[i].SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, h);
            Corners[i].SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
        }
    }

    void Update()
    {
        for (int i=0 ; i<Channels.Length ; ++i)
        {
            Channels[i].texture = SoundController.Get().Sample(i)?IconOn:IconOff;
            Channels[i].color = Color.Lerp(Channels[i].color, _ColorOff, Time.deltaTime*_ColorSpeed);

            if (SoundController.Get().Sample(i))
                Channels[i].color = _ColorOn;
        }

        RectTransform trs = transform as RectTransform;        
        Bass.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, SoundController.Get().Bass*trs.rect.width);
        Treble.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, SoundController.Get().Treble*trs.rect.width);

        trs.anchoredPosition = new Vector2(0.0f, Music.time>=115.0f && Music.time<189.0f?0.0f:-UnityEngine.Screen.height*10.0f);
    }
}
