using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 날짜 : 2021-08-10 PM 10:54:47
// 작성자 : Rito

namespace Rito
{
    /// <summary> 
    /// 내린 눈 지우기
    /// </summary>
    public class SnowBlower : MonoBehaviour
    {
        public GroundSnowPainter groundSnow;
        public float sizeMultiplier = 1f;
        public bool eraseOn = true;

        private void Update()
        {
            if (!eraseOn || groundSnow == null || groundSnow.isActiveAndEnabled == false) return;

            groundSnow.ClearSnow(transform.position, sizeMultiplier * transform.lossyScale.x);
        }
    }
}