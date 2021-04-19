using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-04-19 PM 7:32:55

namespace Rito.DragAndDropRecorder
{
    /// <summary> 기록 대상(드래그 앤 드롭으로 동작) </summary>
    [DisallowMultipleComponent]
    public class DDTarget : MonoBehaviour
    {
        public Vector3 Position => transform.position;

        private Vector3 _beginOrgPos;
        private Vector3 _beginPos;
        private float _beginZ;

        private void OnMouseDown()
        {
            // 기록 시작
            DDRecorder.Instance.SetRecordTarget(this);
            DDRecorder.Instance.BeginRecord();

            // 위치 계산
            _beginOrgPos = transform.position;
            _beginZ = Camera.main.transform.InverseTransformPoint(_beginOrgPos).z;

            Vector3 mPos = GetMousePosition(_beginZ);
            _beginPos = Camera.main.ScreenToWorldPoint(mPos);
        }

        private void OnMouseDrag()
        {
            Vector3 mPos = GetMousePosition(_beginZ);
            Vector3 targetPos = Camera.main.ScreenToWorldPoint(mPos);

            transform.position = _beginOrgPos + (targetPos - _beginPos);
        }

        private void OnMouseUp()
        {
            DDRecorder.Instance.EndRecord();
        }

        private Vector3 GetMousePosition(float zPos)
        {
            Vector3 mPos = Input.mousePosition;
            mPos.z = zPos;
            return mPos;
        }
    }
}