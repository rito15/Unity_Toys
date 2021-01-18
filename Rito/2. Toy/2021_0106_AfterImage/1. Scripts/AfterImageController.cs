using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 설명 : 
public class AfterImageController : MonoBehaviour
{
    public Material _afterImageMaterial;
    public SkinnedMeshRenderer[] _skinnedMeshRendererArray;

    [Range(1, 20)]
    public int _imageCount = 10;        // 생성되는 잔상 개수
    [Range(0.1f, 2f)]
    public float _remainTime = 1.5f;    // 잔상 지속 시간
    [Range(0.01f, 2f)]
    public float _colorTransitionSpeed = 1f; // 잔상 색상 변경 속도

    public bool _createImageOnlyWhenMove = false; // 움직일 때만 잔상 남기기

    private List<SMRAfterImageCreator> aicList;
    private Vector3 _prevPos;

    private void Start()
    {
        if (!CheckVaidation())
        {
            Debug.Log("Skinned Mesh Renderer를 등록해주세요");
            return;
        }

        aicList = new List<SMRAfterImageCreator>();

        foreach (var smr in _skinnedMeshRendererArray)
        {
            var aic = gameObject.AddComponent<SMRAfterImageCreator>();
            aic.Setup(smr, _imageCount, _remainTime, _afterImageMaterial);
            aic.Create();
            aic.isCreating = true;
            aic.colorTransitionSpeed = _colorTransitionSpeed;

            aicList.Add(aic);
        }
        _prevPos = transform.position;
    }

    // 움직일 때만 잔상을 남기고 싶은 경우
    private void Update()
    {
        if (!CheckVaidation()) return;
        if (!_createImageOnlyWhenMove)
        {
            SetCreatingState(true);
            return;
        }

        Vector3 curPos = transform.position;
        bool isMoving = ( Vector3.Magnitude(curPos - _prevPos) > 0.05f );
        _prevPos = transform.position;

        SetCreatingState(isMoving);
    }

    private bool CheckVaidation()
    {
        if (_skinnedMeshRendererArray == null || _skinnedMeshRendererArray.Length == 0)
        {
            return false;
        }

        foreach (var item in _skinnedMeshRendererArray)
        {
            if (item == null)
                return false;
        }

        return true;
    }

    private void SetCreatingState(bool value)
    {
        foreach (var aic in aicList)
            aic.isCreating = value;
    }
}
