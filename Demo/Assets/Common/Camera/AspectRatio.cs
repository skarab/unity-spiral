using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AspectRatio : MonoBehaviour
{
    public Canvas RenderCanvas;
    public RawImage RenderFrame;

    void Awake()
    {
        int width = (int)RenderCanvas.pixelRect.width;
        int height = (int)(width / 2.20f);

        if (height> RenderCanvas.pixelRect.height)
        {
            height = (int)RenderCanvas.pixelRect.height;
            width = (int)(height * 2.20f);
        }

        RenderFrame.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        RenderFrame.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

}
