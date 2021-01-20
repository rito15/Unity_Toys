#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// 날짜 : 2021-01-10 PM 5:13:48
// 작성자 : Rito

namespace Rito.MeshGenerator
{
    using static MeshGeneratorEditorHelper;

    [CustomEditor(typeof(SnowGroundMeshGenerator))]
    public class SnowGroundMeshGeneratorEditor : PerlinNoiseMeshGeneratorEditor
    {
        public SnowGroundMeshGenerator sg;

        private void OnEnable()
        {
            pn = AssetDatabase.Contains(target) ? null : (PerlinNoiseMeshGenerator)target;
            sg = pn as SnowGroundMeshGenerator;
        }

        public override void OnInspectorGUI()
        {
            if (sg == null)
                return;
            Color oldTextColor = GUI.contentColor;
            Color oldBgColor = GUI.backgroundColor;

            EditorGUILayout.Space();

            DrawPerlinProperties();
            DrawSnowProperties();
            DrawPerlinButtons();

            GUI.contentColor = oldTextColor;
            GUI.backgroundColor = oldBgColor;
        }

        private void DrawSnowProperties()
        {
            // 발자국 남기기
            sg._allowFootPrint = EditorGUILayout.Toggle("Allow Footprint", sg._allowFootPrint);
            if (sg._allowFootPrint)
            {
                sg._footPrintDepth =
                    EditorGUILayout.Slider("FootPrint Depth", sg._footPrintDepth, 0.01f, 1.0f);

                EditorGUILayout.Space();

                // 발자국 자동 채우기
                sg._autoAccumulateFootprint =
                    EditorGUILayout.Toggle("Auto Accumulation", sg._autoAccumulateFootprint);

                if (sg._autoAccumulateFootprint)
                {
                    sg._autoAccumCycle =
                        EditorGUILayout.Slider("Accumulation Cycle", sg._autoAccumCycle, 0.01f, 1.0f);

                    sg._autoAccumSpeed =
                        EditorGUILayout.Slider("Accumulation Speed", sg._autoAccumSpeed, 1.0f, 10.0f);
                }
            }
        }
    }
}
#endif