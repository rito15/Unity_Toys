using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-02-08 PM 9:17:45
// 작성자 : Rito

namespace Rito.MouseEvents.Demo
{
    public class MouseEventDataReceiver : MonoBehaviour,
        IMouseEnterData, IMouseExitData, IMouseDownData,
        IMouseUpData, IMouseOverData, IMouseClickData, IMouseDragData
    {
        private MeshRenderer _mr;
        private MaterialPropertyBlock _mpb;

        private void Start()
        {
            TryGetComponent(out _mr);
            _mpb = new MaterialPropertyBlock();
        }

        void IMouseEnterData.OnMouseEnterAction(Vector3 mousePoint)
        {
            ChangeColor(Color.red);
            Debug.Log($"Enter : {name}, {mousePoint}");
        }

        void IMouseExitData.OnMouseExitAction(Vector3 mousePoint)
        {
            ChangeColor(Color.white);
            Debug.Log($"Exit : {name}, {mousePoint}");
        }

        void IMouseOverData.OnMouseOverAction(Vector3 mousePoint)
        {
            Debug.Log($"Over : {name}, {mousePoint}");
        }

        void IMouseDownData.OnMouseDownAction(int mouseButton, Vector3 mousePoint)
        {
            ChangeColor(Color.blue);
            Debug.Log($"Down[{mouseButton}] : {name}, {mousePoint}");
        }

        void IMouseUpData.OnMouseUpAction(int mouseButton, Vector3 mousePoint)
        {
            ChangeColor(Color.green);
            Debug.Log($"Up[{mouseButton}] : {name}, {mousePoint}");
        }

        void IMouseClickData.OnMouseClickAction(int mouseButton, Vector3 mousePoint)
        {
            Debug.Log($"Click[{mouseButton}] : {name}, {mousePoint}");
        }

        void IMouseDragData.OnMouseDragAction(int mouseButton, Vector3 mousePoint)
        {
            Debug.Log($"Drag[{mouseButton}] : {name}, {mousePoint}");

            transform.position
                = new Vector3(mousePoint.x, mousePoint.y, transform.position.z);
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