using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-03-11 AM 3:27:23
// 작성자 : Rito

namespace Rito
{
    [DisallowMultipleComponent]
    public class DontDestroy : MonoBehaviour
    {
        public HideFlags _hideflags;

        private void OnValidate()
        {
            this.hideFlags = _hideflags;
        }

        private void Awake()
        {
            Debug.Log("Here");
        }
    }
}