using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2021. 03. 02. 03:02
// 작성자 : Rito
// 게임오브젝트가 카메라로부터 일정거리를 유지한 채 마우스 커서를 따라가게 한다.

namespace Rito
{
    public class MouseChaser : MonoBehaviour
    {
        // 카메라로부터의 거리
        public float _distanceFromCamera = 10f;

        [Range(0.01f, 1.0f)]
        public float _ChasingSpeed = 0.1f;

        private Vector3 _mousePos;
        private Vector3 _nextPos;

        private void OnValidate()
        {
            if (_distanceFromCamera < 0f)
                _distanceFromCamera = 0f;
        }

        void Update()
        {
            _mousePos = Input.mousePosition;
            _mousePos.z = _distanceFromCamera;

            _nextPos = Camera.main.ScreenToWorldPoint(_mousePos);
            transform.position = Vector3.Lerp(transform.position, _nextPos, _ChasingSpeed);
        }
    }
}