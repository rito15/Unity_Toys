using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-02-08 PM 6:12:57
// 작성자 : Rito

namespace Rito.MouseEvents
{
    public interface IMouseReceiver { }
    public interface IMouseEnter : IMouseReceiver
    {
        void OnMouseEnterAction();
    }
    public interface IMouseExit : IMouseReceiver
    {
        void OnMouseExitAction();
    }
    public interface IMouseOver : IMouseReceiver
    {
        void OnMouseOverAction();
    }

    public interface IMouseDown : IMouseReceiver
    {
        void OnMouseDownAction(int mouseButton);
    }
    public interface IMouseUp : IMouseReceiver
    {
        void OnMouseUpAction(int mouseButton);
    }
    public interface IMouseClick : IMouseReceiver
    {
        void OnMouseClickAction(int mouseButton);
    }
    public interface IMouseDrag : IMouseReceiver
    {
        void OnMouseDragAction(int mouseButton);
    }


    public interface IMouseEnterData : IMouseReceiver
    {
        void OnMouseEnterAction(Vector3 mousePoint);
    }
    public interface IMouseExitData : IMouseReceiver
    {
        void OnMouseExitAction(Vector3 mousePoint);
    }
    public interface IMouseOverData : IMouseReceiver
    {
        void OnMouseOverAction(Vector3 mousePoint);
    }

    public interface IMouseDownData : IMouseReceiver
    {
        void OnMouseDownAction(int mouseButton, Vector3 mousePoint);
    }
    public interface IMouseUpData : IMouseReceiver
    {
        void OnMouseUpAction(int mouseButton, Vector3 mousePoint);
    }
    public interface IMouseClickData : IMouseReceiver
    {
        void OnMouseClickAction(int mouseButton, Vector3 mousePoint);
    }
    public interface IMouseDragData : IMouseReceiver
    {
        void OnMouseDragAction(int mouseButton, Vector3 mousePoint);
    }
}