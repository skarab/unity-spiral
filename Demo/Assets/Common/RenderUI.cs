using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RenderUI : MonoBehaviour
{
    private const int _MaxLines = 12;
    private const float _ScrollerSpeed = 28.0f;

    public Canvas RenderCanvas;
    public TMPro.TextMeshProUGUI TxtUI;
    public TMPro.TextMeshProUGUI Scroller;

    private string _Txt = "";
    private float _Len = 0.0f;
    private float _Timer = 0.0f;
    private float _Speed;
    private RectTransform _TV;
    private float _X;
    private bool _TVEnabled = false;
    private string _ScrollerTxt = "";
    private float _ScrollerLen = 0.0f;
    private const int _ScrollerMaxLines = 40;

    private static RenderUI _Instance = null;
    
    private void Awake()
    {
        _Instance = this;
        _Speed = 23.5f;

        _TV = TxtUI.transform.parent as RectTransform;
        _TV.anchoredPosition = new Vector2(_TV.anchoredPosition.x*UnityEngine.Screen.width/1920.0f, 0.0f);
        _X = _TV.anchoredPosition.x;
        _TV.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _TV.rect.width*UnityEngine.Screen.width/1920.0f);
        _TV.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _TV.rect.height*UnityEngine.Screen.width/1920.0f);
        _TV.anchoredPosition = new Vector2(_X-_TV.rect.width*4.0f, 0.0f);
        _TV.localPosition = new Vector3(_TV.localPosition.x, _TV.localPosition.y, _TV.localPosition.z*UnityEngine.Screen.width/1920.0f);
        
        TxtUI.fontSize = 27.0f*UnityEngine.Screen.width/1920.0f;

        Scroller.fontSize = 15.62f*UnityEngine.Screen.width/1920.0f;
    }

    public static RenderUI Get()
    {
        return _Instance;
    }

    public void Print(string txt)
    {
        if (_Txt.Length>0)
            _Txt += "\n";
        _Txt += txt;
    }

    public void Clear()
    {
        _Txt = "";
        _Len = 0.0f;
    }
    
    public void PrintScroller(string txt)
    {
        _ScrollerTxt = txt;
    }

    public void SetColor(Color color)
    {
        TxtUI.color = color;
    }

    public void EnableTV(bool enabled)
    {
        _TVEnabled = enabled;
    }
    
    void Update()
    {        
        _Timer += Time.deltaTime;

        Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
        RenderCanvas.worldCamera = Array.Find(cameras, c=>c.gameObject.name!="EmptyCamera");
        RenderCanvas.planeDistance = 0.42f;

        _Len = Mathf.Clamp(_Len+Time.deltaTime*_Speed, 0.0f, (float)_Txt.Length);
        string str = _Txt.Substring(0, (int)_Len);
        int count = 0;
        for (int i=str.Length-1 ; i>0 ; --i)
        {
            if (str[i]=='\n')
            {
                ++count;
                if (count>=_MaxLines)
                {
                    str = str.Substring(i+1);
                    break;
                }
            }   
        }

        if (str.Length>0 && Mathf.Sin(_Timer*14.0f)>0.0f)
            str += "_";
        TxtUI.text = str;

        _TV.anchoredPosition = new Vector2(Mathf.Lerp(_TV.anchoredPosition.x, _TVEnabled?_X:_X-_TV.rect.width*4.0f, Time.deltaTime*4.0f), 0.0f);
    
    
        _ScrollerLen = Mathf.Clamp(_ScrollerLen+Time.deltaTime*_ScrollerSpeed, 0.0f, (float)_ScrollerTxt.Length);
        str = _ScrollerTxt.Substring(0, (int)_ScrollerLen);
        count = 0;
        for (int i=str.Length-1 ; i>0 ; --i)
        {
            if (str[i]=='\n')
            {
                ++count;
                if (count>=_ScrollerMaxLines)
                {
                    str = str.Substring(i+1);
                    break;
                }
            }   
        }

        if (str.Length>0 && str[str.Length-1]!='\n' && Mathf.Sin(_Timer*14.0f)>0.0f)
            str += "_";
        Scroller.text = str;    
    }
    
}
