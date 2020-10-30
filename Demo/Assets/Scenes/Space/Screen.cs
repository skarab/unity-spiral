using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Screen : MonoBehaviour
{
    public RawImage RenderRect;
    public Camera Cam;
    public Material ScreenMat;
    public Material ScreenProjMat;
    public RectTransform BassRect;
    public RectTransform TrebleRect;
    public RectTransform SquareRect;
    public TMPro.TextMeshPro TimeTxt;
    public TMPro.TextMeshPro Searching;
    public TMPro.TextMeshPro[] RndTxts;
    public TMPro.TextMeshPro[] RndInts;
    public TMPro.TextMeshPro BigTxt;
    public RectTransform Line;
    public AudioSource Music;

    private RenderTexture Tex;

    void Awake()
    {
        int w = (int)(RenderRect.rectTransform.rect.width*1.0f);
        int h = (int)(w*0.4f);

        Tex = new RenderTexture(w, h, 24, RenderTextureFormat.ARGB32);
        Tex.Create();
       
        Cam.targetTexture = Tex;
        ScreenMat.SetTexture("_BaseColorMap", Tex);
        ScreenProjMat.SetTexture("_UnlitColorMap", Tex);
        ScreenProjMat.SetTexture("_EmissiveColorMap", Tex);
    }

    void Update()
    {
        RectTransform snd = BassRect.parent as RectTransform;
        BassRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (snd.rect.width-BassRect.anchoredPosition.x*2)*SoundController.Get().Bass);
        TrebleRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (snd.rect.width-TrebleRect.anchoredPosition.x*2)*SoundController.Get().Treble);
        SquareRect.Rotate(Vector3.forward, Time.deltaTime*100.0f*(0.3f+SoundController.Get().Bass));
        TimeTxt.text = SoundController.Get().Music.time.ToString();

        Line.Rotate(Vector3.forward, Time.deltaTime*1000.0f*(0.3f+SoundController.Get().Bass));
        Line.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, SquareRect.rect.width*4.0f*(0.3f+SoundController.Get().Bass));
        
        Searching.text = Music.time>70.0f?"SYSTEM FAILURE !!!":"Searching...";
        Searching.enabled = ((int)(Time.time*4.0f))%2==0;
        BigTxt.enabled = ((int)(Time.time*4.0f))%2==0;

        string bigtxt = "unknown objects detected...";
        if (Music.time>70.0f)
        {
            bigtxt = "SYSTEM FAILURE !!!";
        }
        if (SoundController.Get().MidiBass)
        {
            for (int i=0 ; i<9 ; ++i)
            {
                int id = (int)(Random.value*(bigtxt.Length-1));
                bigtxt = bigtxt.Substring(0, id)+(char)('0'+i)+bigtxt.Substring(id+1);
            }
        }
        BigTxt.text = bigtxt;

        for (int i=0 ; i<RndTxts.Length ; ++i)
        {
            if (((int)(Time.time*2.0f+i))%4==0)
            {
                string txt = "";
                for (int j=0 ; j<16 ; ++j)
                    txt += (char)('a'+(int)(Random.value*26.0f));
                RndTxts[i].text = txt;
                RndTxts[i].enabled = true;
            }
            else RndTxts[i].enabled = false;
        }

        for (int i=0 ; i<RndInts.Length ; ++i)
        {
            if (((int)(Time.time*2.0f+i))%2==0)
            {
                string txt = "0.";
                for (int j=0 ; j<8 ; ++j)
                    txt += '0'+(int)(Random.value*9.0f);
                RndInts[i].text = txt;
                RndInts[i].enabled = true;
            }
            else RndInts[i].enabled = false;
        }

        
    }
}
