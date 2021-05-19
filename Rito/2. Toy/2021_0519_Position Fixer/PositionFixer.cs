using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-05-19 PM 7:53:20
// 작성자 : Rito

public class PositionFixer : MonoBehaviour
{
    /// <summary> 위치 고정할지 여부 </summary>
    public bool _activated;

    /// <summary> 위치 고정/해제 기능 키 </summary>
    public KeyCode _functionKey = KeyCode.Space;

    private bool _prevActivated = false;
    private Vector3 _fixedPosition;

    private void OnValidate()
    {
        if (!_prevActivated && _activated)
            _fixedPosition = transform.position;

        _prevActivated = _activated;
    }

    private void Update()
    {
        if (Input.GetKeyDown(_functionKey))
            Fix(!_activated);

        if (_activated)
            transform.position = _fixedPosition;
    }

    /// <summary> 고정 / 해제 </summary>
    public void Fix(bool isTrue)
    {
        _fixedPosition = transform.position;
        _activated = isTrue;
    }
}