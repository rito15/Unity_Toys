using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// 날짜 : 2021-05-22 PM 9:27:14
// 작성자 : Rito

namespace Rito
{
    [DisallowMultipleComponent]
    public partial class MaterialValueChanger : MonoBehaviour
    {
        /***********************************************************************
        *                               Class Definitions
        ***********************************************************************/
        #region .

        private abstract class PropertyData<T> where T : struct
        {
            public string name; 
            public int index;   // 해당 프로퍼티의 쉐이더 내 인덱스

            public T Value => _value;
            public T MinValue
            {
                get => _minValue;
                set
                {
                    _minValue = value;
                    CalculateDeltaCoef();
                }
            }
            public T MaxValue
            {
                get => _maxValue;
                set
                {
                    _maxValue = value;
                    CalculateDeltaCoef();
                }
            }

            public float Duration // 지속 시간
            {
                get => _duration;
                set
                {
                    _duration = value;
                    CalculateDeltaCoef();
                }
            }
            public float Progress => _progress; // 현재 진행 시간

            protected T _value;
            protected T _minValue;
            protected T _maxValue;
            protected T _deltaCoef; // 매 프레임 더할 값

            protected int _direction = 1;   // 값 변화 방향 및 크기
            protected float _duration = 1f;
            protected float _progress = 0f;

            public void SetNewProperty(string name, int index, float duration)
            {
                this.name = name;
                this.index = index;
                this._duration = duration;
            }

            public void SetValues(in T value, in T minValue, in T maxValue)
            {
                this._value = value;
                this._minValue = minValue;
                this._maxValue = maxValue;

                CalculateDeltaCoef();
            }

            /// <summary> 매 프레임 호출 </summary>
            public void Update(in float deltaTime)
            {
                if(_direction == 0) return;
                if(!InRange(_progress, 0f, _duration)) return;

                _progress += deltaTime * _direction;
                if (_progress < 0) _progress = 0;
                else if(_progress > _duration) _progress = _duration;

                // Value = min + (max - min) * (progress / duration)
                //       = deltaCoef * progress
                _value = Mul(_deltaCoef, _progress);

                // Set Material Property Value?
            }

            public void Run(bool isPositive = true)
            {
                _direction = isPositive ? 1 : -1;
            }

            public void Stop()
            {
                _direction = 0;
            }

            protected bool InRange(in float value, in float minValue, in float maxValue)
            {
                return (minValue < value && value < maxValue);
            }

            private void CalculateDeltaCoef()
            {
                // deltaCoef = (max - min) / duration
                _deltaCoef = Div(Sub(_maxValue, _minValue), _duration);
            }

            protected abstract T Add(in T a, in T b); // Generic +
            protected abstract T Sub(in T a, in T b); // Generic -
            protected abstract T Mul(in T a, in float b); // Generic *
            protected abstract T Div(in T a, in float b); // Generic /
        }

        [Serializable]
        private class FloatPropertyData : PropertyData<float>
        {
            public FloatPropertyData(string name, int index, float duration)
            {
                SetNewProperty(name, index, duration);
                SetValues(0f, 0f, 1f);
            }

            protected override float Add(in float a, in float b) => a + b;
            protected override float Sub(in float a, in float b) => a - b;
            protected override float Mul(in float a, in float b) => a * b;
            protected override float Div(in float a, in float b) => a / b;
        }

        [Serializable]
        private class Vector4PropertyData : PropertyData<Vector4>
        {
            public Vector4PropertyData(string name, int index, float duration)
            {
                SetNewProperty(name, index, duration);
                SetValues(Vector4.zero, Vector4.zero, Vector4.one);
            }

            protected override Vector4 Add(in Vector4 a, in Vector4 b) => a + b;
            protected override Vector4 Sub(in Vector4 a, in Vector4 b) => a - b;
            protected override Vector4 Mul(in Vector4 a, in float b) => a * b;
            protected override Vector4 Div(in Vector4 a, in float b) => a / b;
        }

        [Serializable]
        private class ColorPropertyData : PropertyData<Color>
        {
            public ColorPropertyData(string name, int index, float duration)
            {
                SetNewProperty(name, index, duration);
                SetValues(Color.black, Color.white, Color.white);
            }

            protected override Color Add(in Color a, in Color b) => a + b;
            protected override Color Sub(in Color a, in Color b) => a - b;
            protected override Color Mul(in Color a, in float b) => a * b;
            protected override Color Div(in Color a, in float b) => a / b;
        }

        #endregion

        [SerializeField] private MeshRenderer mr;
        [SerializeField] private List<FloatPropertyData> floatPropList = new List<FloatPropertyData>();
        [SerializeField] private List<Vector4PropertyData> vec4PropList = new List<Vector4PropertyData>();
        [SerializeField] private List<ColorPropertyData> colorPropList = new List<ColorPropertyData>();

        // (프로퍼티 타입, 해당 타입 리스트 내부의 인덱스)
        private List<(ShaderPropertyType propType, int index)> propertyList
            = new List<(ShaderPropertyType propType, int index)>();

        private Shader shader;

        private void Start()
        {
            shader = mr.sharedMaterial.shader;
            int propertyCount = shader.GetPropertyCount();

            int j = 0;
            for (int i = 0; i < propertyCount; i++)
            {
                var pType = shader.GetPropertyType(i);
                switch (pType)
                {
                    case ShaderPropertyType.Float:
                    case ShaderPropertyType.Range:
                    case ShaderPropertyType.Vector:
                    case ShaderPropertyType.Color:
                        break;

                    default:
                        continue;
                }

                string str = $"[{j++}] ";
                str += shader.GetPropertyName(i);
                str += ": ";
                str += pType.ToString();
                
                Debug.Log(str);
            }
        }
    }
}