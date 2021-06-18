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
        private partial class Custom : UnityEditor.Editor
        {
            /***********************************************************************
            *                               Inspector Methods
            ***********************************************************************/
            #region .
            private void DrawAdditionGroup()
            {
                // Backgrounds


                // Contents
                EditorGUI.LabelField(GetRect(0.05f, 0.24f, currentY - 2f, 20f), "New Property");
                popupSelected = EditorGUI.Popup(GetRect(0.45f, 0.79f, currentY + 1f, 20f), popupSelected, m.propertyNameArray);
                GUI.Button(GetRect(0.8f, 0.9f, currentY, 20f), "Add");

                NextY(20f);
            }

            private void DrawPropertyList()
            {

            }

            #endregion
            /***********************************************************************
            *                               Drawing Methods
            ***********************************************************************/
            #region .
            // 각각 프로퍼티 타입에 따라 인스펙터 요소 그리기

            private void DrawFloatProperty(FloatPropertyData data, float yPos)
            {
                // Name
                // Value : Slider
                // MinValue : FloatField / MaxValue : FloatField
                // Duration : FloatField

                EditorGUILayout.LabelField(data.Name);
                EditorGUILayout.Slider(data.Value, data.MinValue, data.MaxValue);

            }

            private void DrawVector4Property()
            {

            }

            private void DrawColorProperty()
            {

            }

            #endregion
            /***********************************************************************
            *                               Tiny Methods
            ***********************************************************************/
            #region .
            private GUILayoutOption GetWidth(float ratio)
            {
                return GUILayout.Width(currentViewWidth * ratio);
            }

            private float GetX(float ratio)
            {
                return currentViewWidth * ratio;
            }

            private void NextY(float height)
            {
                currentY += height;
            }

            private Rect GetRect(in float xRatioBegin, in float xRatioEnd, in float offsetY, in float height)
            {
                return new Rect(GetX(xRatioBegin), currentY + offsetY, GetX(xRatioEnd - xRatioBegin), height);
            }

            #endregion
            /***********************************************************************
            *                               Methods
            ***********************************************************************/
            #region .
            // 타입 선택해서 추가하면 알아서 해당 타입의 리스트에 새로운 요소 추가하고
            // propertyList에서 간접 참조 연결
            private void AddProperty(in PropertyInfo prop)
            {
                int index;
                switch (prop.type)
                {
                    case ShaderPropertyType.Float:
                    case ShaderPropertyType.Range:
                        index = m.floatPropList.Count;
                        m.floatPropList.Add(new FloatPropertyData(prop.name));
                        break;

                    case ShaderPropertyType.Vector:
                        index = m.vec4PropList.Count;
                        m.vec4PropList.Add(new Vector4PropertyData(prop.name));
                        break;

                    case ShaderPropertyType.Color:
                        index = m.colorPropList.Count;
                        m.colorPropList.Add(new ColorPropertyData(prop.name));
                        break;

                    default:
                        return;
                }

                m.inspectorPropertyList.Add(new PropertyIndex(prop.name, prop.type, index));
            }

            #endregion
        }
    }
}