using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-04-19 PM 7:53:23

namespace Rito.DragAndDropRecorder
{
    public struct RecordData
    {
        public float elapsedTime;
        public Vector3 position;

        public RecordData(float time, Vector3 position)
        {
            elapsedTime = time;
            this.position = position;
        }
    }
}