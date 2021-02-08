using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-02-08 PM 5:54:44
// 작성자 : Rito

namespace Rito.MouseEvents.Demo
{
    //static class Debug
    //{
    //    public static void Log<T>(T a) { }
    //}

    public class MouseEventReceiver : MonoBehaviour, 
        IMouseEnter, IMouseExit, IMouseDown, IMouseUp, IMouseOver, IMouseClick, IMouseDrag
    {
        private MeshRenderer _mr;
        private MaterialPropertyBlock _mpb;

        private void Start()
        {
            TryGetComponent(out _mr);
            _mpb = new MaterialPropertyBlock();
        }

        void IMouseEnter.OnMouseEnterAction()
        {
            ChangeColor(Color.red);
        }

        void IMouseExit.OnMouseExitAction()
        {
            ChangeColor(Color.white);
        }

        void IMouseOver.OnMouseOverAction()
        {
            Debug.Log($"Over : {name}");
        }

        void IMouseDown.OnMouseDownAction(int mouseButton)
        {
            ChangeColor(Color.blue);
            Debug.Log($"Down[{mouseButton}] : {name}");
        }

        void IMouseUp.OnMouseUpAction(int mouseButton)
        {
            ChangeColor(Color.green);
            Debug.Log($"Up[{mouseButton}] : {name}");
        }

        void IMouseClick.OnMouseClickAction(int mouseButton)
        {
            Debug.Log($"Click[{mouseButton}] : {name}");
        }

        void IMouseDrag.OnMouseDragAction(int mouseButton)
        {
            Debug.Log($"Drag[{mouseButton}] : {name}");
        }

        private void ChangeColor(in Color color)
        {
            _mr.GetPropertyBlock(_mpb);
            _mpb.SetColor("_Color", color);
            _mr.SetPropertyBlock(_mpb);

            //_mr.material.color = color;
        }
    }
}