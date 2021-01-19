using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-19 PM 5:01:57
// 작성자 : Rito

[ExecuteInEditMode]
public class PixelationCamera : MonoBehaviour
{
    [Range(1, 100)]
    public int pixelate = 1;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        source.filterMode = FilterMode.Point;
        RenderTexture resultTexture = RenderTexture.GetTemporary(source.width / pixelate, source.height / pixelate, 0, source.format);
        resultTexture.filterMode = FilterMode.Point;

        Graphics.Blit(source, resultTexture);
        Graphics.Blit(resultTexture, destination);
        RenderTexture.ReleaseTemporary(resultTexture);
    }

    private void OnGUI()
    {
        string text = $"Pixelate : {pixelate,3}";

        Rect textRect = new Rect(60f, 60f, 440f, 100f);
        Rect boxRect = new Rect(40f, 40f, 460f, 120f);

        GUIStyle boxStyle = GUI.skin.box;
        GUI.Box(boxRect, "", boxStyle);

        GUIStyle textStyle = GUI.skin.label;
        textStyle.fontSize = 70;
        GUI.TextField(textRect, text, 50, textStyle);
    }
}