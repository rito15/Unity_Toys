using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

// 날짜 : 2021-05-23 AM 1:52:02
// 작성자 : Rito

namespace Rito
{
    public partial class MaterialValueChanger : MonoBehaviour
    {
        [CustomEditor(typeof(MaterialValueChanger))]
        private class Custom : UnityEditor.Editor
        {
            private MaterialValueChanger m;
            private float currentViewWidth;

            private void OnEnable()
            {
                if(m == null)
                    m = target as MaterialValueChanger;
            }

            public override void OnInspectorGUI()
            {
                currentViewWidth = EditorGUIUtility.currentViewWidth;

                base.OnInspectorGUI();
            }

            // 타입 선택해서 추가하면 알아서 해당 타입의 리스트에 새로운 요소 추가하고
            // propertyList에서 간접 참조 연결
            private void AddProperty(ShaderPropertyType propType)
            {
                int index = -1;
                switch (propType)
                {
                    case ShaderPropertyType.Float:
                    case ShaderPropertyType.Range:
                        index = m.floatPropList.Count;
                        // add to floatPropList
                        break;

                    case ShaderPropertyType.Vector:
                        index = m.vec4PropList.Count;
                        // add to vec4PropList
                        break;

                    case ShaderPropertyType.Color:
                        index = m.colorPropList.Count;
                        // add to colorPropList
                        break;

                    default:
                        return;
                }

                m.propertyList.Add((propType, index));
            }

            // 각각 프로퍼티 타입에 따라 인스펙터 요소 그리기

            private void DrawFloatProperty()
            {

            }

            private void DrawVector4Property()
            {

            }

            private void DrawColorProperty()
            {

            }
        }
    }
}