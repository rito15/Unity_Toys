using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-18 PM 4:33:20
// 작성자 : Rito

public sealed class MeshAfterImageFader : AfterImageFaderBase
{
    public List<Mesh> TargetMeshList { get; private set; }

    /***********************************************************************
    *                               Public Methods
    ***********************************************************************/
    #region .
    public override void Setup(Array targetArray, AfterImageData data, AfterImageBase controller)
    {
        TargetTransformList = new List<Transform>();
        TargetMeshList = new List<Mesh>();

        ChildrenTransformList = new List<Transform>();
        ChildrenRendererList = new List<MeshRenderer>();

        MeshFilter[] targetFilters = targetArray as MeshFilter[];
        foreach (var filter in targetFilters)
        {
            TargetMeshList.Add(filter.sharedMesh);
            TargetTransformList.Add(filter.transform);
        }

        Data = data;
        Controller = controller;
        CurrentElapsedTime = 0f;

        CreateChildImages();
    }

    #endregion
    /***********************************************************************
    *                               Protected Methods
    ***********************************************************************/
    #region .
    protected override void CreateChildImages()
    {
        for(int i = 0; i < TargetMeshList.Count; i++)
        {
            GameObject instanceGo = new GameObject("Image");
            Transform instanceTr = instanceGo.transform;

            instanceTr.SetParent(transform);

            var renderer = instanceGo.AddComponent<MeshRenderer>();
            var filter = instanceGo.AddComponent<MeshFilter>();

            filter.mesh = TargetMeshList[i];
            renderer.material = Data.Mat;

            ChildrenRendererList.Add(renderer);
            ChildrenTransformList.Add(instanceTr);
        }
    }

    #endregion

}