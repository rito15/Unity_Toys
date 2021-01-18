using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-18 PM 7:06:31
// 작성자 : Rito

public sealed class SkinnedMeshAfterImageFader : AfterImageFaderBase
{
    private List<SkinnedMeshRenderer> TargetSmrList { get; set; }
    private List<MeshFilter> ChildrenFilterList { get; set; }

    /***********************************************************************
    *                               Public Methods
    ***********************************************************************/
    #region .
    public override void Setup(Array targetArray, AfterImageData data, AfterImageBase controller)
    {
        TargetTransformList = new List<Transform>();
        TargetSmrList = new List<SkinnedMeshRenderer>();

        ChildrenTransformList = new List<Transform>();
        ChildrenFilterList = new List<MeshFilter>();
        ChildrenRendererList = new List<MeshRenderer>();

        SkinnedMeshRenderer[] targetSmrs = targetArray as SkinnedMeshRenderer[];
        foreach (var smr in targetSmrs)
        {
            TargetTransformList.Add(smr.transform);
            TargetSmrList.Add(smr);
        }

        Data = data;
        Controller = controller;
        CurrentElapsedTime = 0f;

        CreateChildImages();
    }

    public override void WakeUp(in Color color)
    {
        for (int i = 0; i < ChildrenFilterList.Count; i++)
        {
            TargetSmrList[i].BakeMesh(ChildrenFilterList[i].mesh);
        }
        base.WakeUp(color);
    }

    #endregion
    /***********************************************************************
    *                               Protected Methods
    ***********************************************************************/
    #region .
    protected override void CreateChildImages()
    {
        for (int i = 0; i < TargetSmrList.Count; i++)
        {
            GameObject instanceGo = new GameObject("Image");
            Transform instanceTr = instanceGo.transform;

            instanceTr.SetParent(transform);

            var filter = instanceGo.AddComponent<MeshFilter>();
            var renderer = instanceGo.AddComponent<MeshRenderer>();

            renderer.material = Data.Mat;

            ChildrenRendererList.Add(renderer);
            ChildrenFilterList.Add(filter);
            ChildrenTransformList.Add(instanceTr);
        }
    }

    #endregion
}