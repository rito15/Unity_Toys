using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 날짜 : 2021-08-07 AM 1:12:20
// 작성자 : Rito

namespace Rito
{
    /// <summary> 화면에 마우스 드래그로 사각형 선택 영역 표시하기 </summary>
    public class ScreenDragSelection : MonoBehaviour
    {
        private Vector2 mPosCur;
        private Vector2 mPosBegin;
        private Vector2 mPosMin;
        private Vector2 mPosMax;
        private bool showSelection;

        private void Update()
        {
            showSelection = Input.GetMouseButton(0);
            if (!showSelection) return;

            mPosCur = Input.mousePosition;
            mPosCur.y = Screen.height - mPosCur.y; // Y 좌표(상하) 반전

            if (Input.GetMouseButtonDown(0))
            {
                mPosBegin = mPosCur;
            }

            mPosMin.x = Mathf.Min(mPosCur.x, mPosBegin.x);
            mPosMin.y = Mathf.Min(mPosCur.y, mPosBegin.y);
            mPosMax.x = Mathf.Max(mPosCur.x, mPosBegin.x);
            mPosMax.y = Mathf.Max(mPosCur.y, mPosBegin.y);
        }

        private void OnGUI()
        {
            if (!showSelection) return;
            Rect rect = new Rect();
            rect.min = mPosMin;
            rect.max = mPosMax;

            GUI.Box(rect, "");
        }
    }
}