using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-02-08 PM 8:00:51
// 작성자 : Rito

namespace Rito.MouseEvents.Demo
{
    public class LegacyMouseEventReceiver : MonoBehaviour
    {
        private MeshRenderer _mr;
        private MaterialPropertyBlock _mpb;

        private void Start()
        {
            TryGetComponent(out _mr);
            _mpb = new MaterialPropertyBlock();
        }

        void OnMouseEnter()
        {
            ChangeColor(Color.red);
        }

        void OnMouseExit()
        {
            ChangeColor(Color.white);
        }

        void OnMouseDown()
        {
            ChangeColor(Color.blue);
            Debug.Log($"Mouse Down : {name}");
        }

        void OnMouseUp()
        {
            ChangeColor(Color.green);
            Debug.Log($"Mouse Up : {name}");
        }

        void OnMouseOver()
        {
            Debug.Log($"Over : {name}");
        }


        private void OnMouseDrag()
        {
            Debug.Log($"Drag : {name}");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
                Destroy(this);
        }

        private void ChangeColor(in Color color)
        {
            _mr.GetPropertyBlock(_mpb);
            _mpb.SetColor("_Color", color);
            _mr.SetPropertyBlock(_mpb);
        }
    }
}