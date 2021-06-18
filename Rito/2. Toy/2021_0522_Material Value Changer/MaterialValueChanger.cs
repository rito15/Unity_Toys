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
            public string Name => _name;
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

            protected string _name;
            protected int _direction = 1;   // 값 변화 방향 및 크기
            protected float _duration = 1f;
            protected float _progress = 0f;

            public void SetNewProperty(string name, float duration)
            {
                this._name = name;
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
            public FloatPropertyData(string name, float duration = 1f)
            {
                SetNewProperty(name, duration);
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
            public Vector4PropertyData(string name, float duration = 1f)
            {
                SetNewProperty(name, duration);
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
            public ColorPropertyData(string name, float duration = 1f)
            {
                SetNewProperty(name, duration);
                SetValues(Color.black, Color.white, Color.white);
            }

            protected override Color Add(in Color a, in Color b) => a + b;
            protected override Color Sub(in Color a, in Color b) => a - b;
            protected override Color Mul(in Color a, in float b) => a * b;
            protected override Color Div(in Color a, in float b) => a / b;
        }

        #endregion
        /***********************************************************************
        *                               Property List
        ***********************************************************************/
        #region .
        private readonly struct PropertyInfo
        {
            public readonly string name;
            public readonly ShaderPropertyType type;

            public PropertyInfo(string name, ShaderPropertyType type)
            {
                this.name = name;
                this.type = type;
            }
        }
        private readonly struct PropertyIndex
        {
            public readonly string name;
            public readonly ShaderPropertyType type;
            public readonly int index;

            public PropertyIndex(string name, ShaderPropertyType type, int index)
            {
                this.name = name;
                this.type = type;
                this.index = index;
            }
        }

        private readonly List<PropertyInfo> propertyInfoList = new List<PropertyInfo>(10);
        private string[] propertyNameArray;
        //private readonly Dictionary<string, int> propertyIndexDict = new Dictionary<string, int>();

        // Inspector Property List
        private readonly List<PropertyIndex> inspectorPropertyList = new List<PropertyIndex>();

        #endregion
        /***********************************************************************
        *                               Fields
        ***********************************************************************/
        #region .
        [SerializeField] private MeshRenderer mr;
        [SerializeField] private List<FloatPropertyData> floatPropList = new List<FloatPropertyData>();
        [SerializeField] private List<Vector4PropertyData> vec4PropList = new List<Vector4PropertyData>();
        [SerializeField] private List<ColorPropertyData> colorPropList = new List<ColorPropertyData>();


        private Shader shader;
        private MaterialPropertyBlock mpBlock;

        #endregion
        /***********************************************************************
        *                               Private Methods
        ***********************************************************************/
        #region .

        private void MakePropertyList()
        {
            propertyInfoList.Clear();
            //propertyIndexDict.Clear();

            int propertyCount = shader.GetPropertyCount();

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

                propertyInfoList.Add(new PropertyInfo(shader.GetPropertyName(i), pType));
            }

            int len = propertyInfoList.Count;
            propertyNameArray = new string[len];
            for (int i = 0; i < len; i++)
            {
                propertyNameArray[i] = propertyInfoList[i].name;
            }
        }
        private void Init()
        {
            TryGetComponent(out mr);
            shader = mr.sharedMaterial.shader;
        }

        #endregion
        /***********************************************************************
        *                               Unity Events
        ***********************************************************************/
        #region .

        private void Reset()
        {
            propertyInfoList.Clear();
            inspectorPropertyList.Clear();
            floatPropList.Clear();
            vec4PropList.Clear();
            colorPropList.Clear();

            Init();
            MakePropertyList();
        }
        private void Awake()
        {
            mpBlock = new MaterialPropertyBlock();
        }

        #endregion
    }
}