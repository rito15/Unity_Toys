using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-18 PM 4:52:11
// 작성자 : Rito

[Serializable]
public class AfterImageData
{
    [Range(0.1f, 2.0f), Tooltip("잔상 지속 시간")]
    public float duration = 1.0f;

    public string shaderColorName = "_Color";
    public string shaderAlphaName = "_Alpha";

    public Material Mat { get; set; }
}