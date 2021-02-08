using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-02-08 PM 5:54:25
// 작성자 : Rito

namespace Rito.MouseEvents
{
    public class MouseEventCaller : SingletonMonoBehavior<MouseEventCaller>
    {
        public LayerMask _layerFilter = -1;
        public float _rayDistanceMax = 1000f;

        const int MouseMaxIndex = 6;

        /// <summary> 확인할 마우스 버튼 범위 (0 ~ 6) </summary>
        [Range(0, MouseMaxIndex)]
        public int _mouseButtonRange = 2;

        // 0 : Left
        // 1 : Right
        // 2 : Middle
        // 3 ~ 6 : Extra Buttons

        private GameObject _prevObject;
        private GameObject _currentObject;
        private GameObject[] _clickedObjects;
        private List<IMouseReceiver> _prevReceiverList;
        private List<IMouseReceiver> _currentReceiverList;
        private Dictionary<int, List<IMouseReceiver>> _dragListDict;

        private Vector3 _mouseHitPoint;
        private bool _curExist = false;
        private bool _prevExist = false;

        /***********************************************************************
        *                               Unity Events
        ***********************************************************************/
        #region .

        private void Start()
        {
            _prevReceiverList = new List<IMouseReceiver>();
            _currentReceiverList = new List<IMouseReceiver>();
            _clickedObjects = new GameObject[MouseMaxIndex + 1];

            _dragListDict = new Dictionary<int, List<IMouseReceiver>>();
            for (int i = 0; i <= MouseMaxIndex; i++)
            {
                _dragListDict[i] = new List<IMouseReceiver>();
            }
        }

        private void Update()
        {
            // Init
            _currentObject = GetMousePosGameObject();

            if (_currentObject && (_currentReceiverList.Count == 0 || _currentObject != _prevObject))
            {
                InitMouseReceiverList(_currentObject);
            }

            _curExist  = IsCurrentReceiverExist();
            _prevExist = IsPrevReceiverExist();

            // Events
            if (_curExist)
            {
                InvokeMouseOverEvent();

                if (!_prevExist || _currentObject != _prevObject)
                {
                    InvokeMouseEnterEvent();
                    InvokeMouseExitEvent();
                }

                for (int i = 0; i <= _mouseButtonRange; i++)
                {
                    if (Input.GetMouseButtonDown(i))
                    {
                        InvokeMouseDownEvent(i);
                        _clickedObjects[i] = _currentObject;
                        _dragListDict[i].AddRange(_currentReceiverList);
                    }
                    else if (Input.GetMouseButtonUp(i))
                    {
                        InvokeMouseUpEvent(i);
                        if (_clickedObjects[i] == _currentObject)
                        {
                            InvokeMouseClickEvent(i);
                            _clickedObjects[i] = null;
                        }
                    }
                }
            }

            if (!_curExist && _prevExist)
            {
                InvokeMouseExitEvent();
            }

            // Drag
            for (int i = 0; i <= _mouseButtonRange; i++)
            {
                if (Input.GetMouseButton(i))
                {
                    InvokeMouseDragEvent(i);
                }
                else if (Input.GetMouseButtonUp(i) && _dragListDict[i].Count > 0)
                {
                    _dragListDict[i].Clear();
                }
            }

            // Set Prev
            _prevObject = _currentObject;

            if (_prevExist)
            {
                _prevReceiverList.Clear();
            }

            if (_curExist)
            {
                _prevReceiverList.AddRange(_currentReceiverList);
            }
        }

        #endregion
        /***********************************************************************
        *                               Invoke Methods
        ***********************************************************************/
        #region .
        private void InvokeMouseEnterEvent()
        {
            try
            {
                foreach (var curReceiver in _currentReceiverList)
                {
                    (curReceiver as IMouseEnter)?.OnMouseEnterAction();
                    (curReceiver as IMouseEnterData)?.OnMouseEnterAction(_mouseHitPoint);
                }
            }
            catch (MissingReferenceException)
            {
                Debug.Log("MRE - Enter");
            }
        }

        private void InvokeMouseExitEvent()
        {
            try
            {
                foreach (var prevReceiver in _prevReceiverList)
                {
                    (prevReceiver as IMouseExit)?.OnMouseExitAction();
                    (prevReceiver as IMouseExitData)?.OnMouseExitAction(_mouseHitPoint);
                }
            }
            catch (MissingReferenceException)
            {
                Debug.Log("MRE - Exit");
            }
        }

        private void InvokeMouseOverEvent()
        {
            try
            {
                foreach (var curReceiver in _currentReceiverList)
                {
                    (curReceiver as IMouseOver)?.OnMouseOverAction();
                    (curReceiver as IMouseOverData)?.OnMouseOverAction(_mouseHitPoint);
                }
            }
            catch (MissingReferenceException)
            {
                _currentReceiverList.Clear();
                Debug.Log("MRE - Over");
            }
        }

        private void InvokeMouseDownEvent(int mouseButton)
        {
            try
            {
                foreach (var curReceiver in _currentReceiverList)
                {
                    (curReceiver as IMouseDown)?.OnMouseDownAction(mouseButton);
                    (curReceiver as IMouseDownData)?.OnMouseDownAction(mouseButton, _mouseHitPoint);
                }
            }
            catch (MissingReferenceException)
            {
                Debug.Log("MRE - Down");
            }
        }

        private void InvokeMouseUpEvent(int mouseButton)
        {
            try
            {
                foreach (var curReceiver in _currentReceiverList)
                {
                    (curReceiver as IMouseUp)?.OnMouseUpAction(mouseButton);
                    (curReceiver as IMouseUpData)?.OnMouseUpAction(mouseButton, _mouseHitPoint);
                }
            }
            catch (MissingReferenceException) { }
        }

        private void InvokeMouseClickEvent(int mouseButton)
        {
            try
            {
                foreach (var curReceiver in _currentReceiverList)
                {
                    (curReceiver as IMouseClick)?.OnMouseClickAction(mouseButton);
                    (curReceiver as IMouseClickData)?.OnMouseClickAction(mouseButton, _mouseHitPoint);
                }
            }
            catch (MissingReferenceException)
            {
                Debug.Log("MRE - Click");
            }
        }

        private void InvokeMouseDragEvent(int mouseButton)
        {
            try
            {
                foreach (var curReceiver in _dragListDict[mouseButton])
                {
                    (curReceiver as IMouseDrag)?.OnMouseDragAction(mouseButton);
                    (curReceiver as IMouseDragData)?.OnMouseDragAction(mouseButton, _mouseHitPoint);
                }
            }
            catch (MissingReferenceException)
            {
                _dragListDict[mouseButton].Clear();
            }
        }

        #endregion
        /***********************************************************************
        *                               Methods
        ***********************************************************************/
        #region .
        private GameObject GetMousePosGameObject()
        {
            bool raycast =
                Physics.Raycast(
                    Camera.main.ScreenPointToRay(Input.mousePosition),
                    out var hit, _rayDistanceMax, _layerFilter
                );

            if (raycast)
            {
                _mouseHitPoint = hit.point;
            }

            return raycast ? hit.collider.gameObject : null;
        }

        private void InitMouseReceiverList(GameObject go)
        {
            var monos = go.GetComponents<MonoBehaviour>();

            _currentReceiverList.Clear();

            if (monos.Length == 0) return;

            foreach (var mono in monos)
            {
                if (mono is IMouseReceiver receiver)
                {
                    _currentReceiverList.Add(receiver);
                }
            }
        }

        private bool IsCurrentReceiverExist() =>
            _currentObject != null && _currentReceiverList.Count > 0;
        private bool IsPrevReceiverExist() =>
            _prevObject && _prevReceiverList.Count > 0;

        #endregion
    }
}