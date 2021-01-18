using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-18 PM 4:07:08
// 작성자 : Rito

public abstract class AfterImageFaderBase : MonoBehaviour
{
    protected AfterImageBase Controller { get; set; }
    protected List<Transform> TargetTransformList { get; set; }
    protected List<Transform> ChildrenTransformList { get; set; }
    protected List<MeshRenderer> ChildrenRendererList { get; set; }

    protected AfterImageData Data { get; set; }

    protected float CurrentAlpha { get; set; }

    protected float CurrentElapsedTime { get; set; }

    /// <summary> 현재 알파를 업데이트하는 구간값 </summary>
    protected const float AlphaUpdateInterval = 0.1f;

    /***********************************************************************
    *                               Unity Events
    ***********************************************************************/
    #region .
    private void Update()
    {
        CurrentElapsedTime += Time.deltaTime;

        if (CurrentElapsedTime >= Data.duration * AlphaUpdateInterval)
        {
            CurrentAlpha -= AlphaUpdateInterval;
            SetChildrenAlpha(CurrentAlpha);

            CurrentElapsedTime = 0f;
        }

        if (CurrentAlpha <= 0f)
        {
            CurrentElapsedTime = 0f;
            Sleep();
        }
    }

    #endregion

    /***********************************************************************
    *                               Methods
    ***********************************************************************/
    #region .

    public abstract void Setup(Array targetArray, AfterImageData data, AfterImageBase controller);

    protected abstract void CreateChildImages();


    public virtual void WakeUp(in Color color)
    {
        gameObject.SetActive(true);
        SetChildrenColor(color);
        CurrentAlpha = 1.0f;

        for (int i = 0; i < ChildrenTransformList.Count; i++)
        {
            ChildrenTransformList[i].SetPositionAndRotation(
                TargetTransformList[i].position,
                TargetTransformList[i].rotation
            );
            ChildrenTransformList[i].localScale = TargetTransformList[i].localScale;
        }
    }

    protected void Sleep()
    {
        Controller.SetImageReadyState(this);
        gameObject.SetActive(false);
    }


    protected void SetChildrenColor(in Color color)
    {
        foreach (var renderer in ChildrenRendererList)
        {
            renderer.material.SetVector(Data.shaderColorName,
                new Vector4(color.r, color.g, color.b, 1f));
        }
    }

    protected void SetChildrenAlpha(in float alpha)
    {
        foreach (var renderer in ChildrenRendererList)
        {
            renderer.material.SetFloat(Data.shaderAlphaName, alpha);
        }
    }

    #endregion
}