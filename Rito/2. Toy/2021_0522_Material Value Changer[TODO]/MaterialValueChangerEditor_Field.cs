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
        private partial class Custom : UnityEditor.Editor
        {
            private MaterialValueChanger m;
            private float currentViewWidth;
            private float currentY = 0;

            private int popupSelected = 0;

            private void OnEnable()
            {
                if (m == null)
                    m = target as MaterialValueChanger;

                m.MakePropertyList();
            }

            public override void OnInspectorGUI()
            {
                currentY = 8f;
                currentViewWidth = EditorGUIUtility.currentViewWidth;

                DrawAdditionGroup();

                //using (new EditorGUILayout.HorizontalScope())
                //{
                //    EditorGUILayout.LabelField("New Property", GetWidth(0.2f));
                //    popupSelected = EditorGUILayout.Popup(popupSelected, m.propertyNameArray, GetWidth(0.6f));
                //    if (GUILayout.Button("Add", GetWidth(0.1f)))
                //    {
                //        AddProperty(m.propertyInfoList[popupSelected]);
                //    }
                //}


                for (int i = 0; i < m.inspectorPropertyList.Count; i++)
                {
                    PropertyIndex current = m.inspectorPropertyList[i];

                    switch (current.type)
                    {
                        case ShaderPropertyType.Float:
                        case ShaderPropertyType.Range:
                            DrawFloatProperty(m.floatPropList[current.index], currentY);
                            break;
                    }
                }

                EditorGUILayout.Space(currentY + 12f);
            }
        }
    }
}