#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// 날짜 : 2021-01-10 AM 1:21:43
// 작성자 : Rito

namespace Rito.MeshGenerator
{
    using static MeshGeneratorEditorHelper;

    [CustomEditor(typeof(PerlinNoiseMeshGenerator))]
    public class PerlinNoiseMeshGeneratorEditor : UnityEditor.Editor
    {
        public PerlinNoiseMeshGenerator selected;

        private void OnEnable()
        {
            selected = AssetDatabase.Contains(target) ? null : (PerlinNoiseMeshGenerator)target;
        }

        public override void OnInspectorGUI()
        {
            if (selected == null)
                return;
            Color oldTextColor = GUI.contentColor;
            Color oldBgColor = GUI.backgroundColor;

            EditorGUILayout.Space();

            DrawPerlinProperties();
            DrawPerlinButtons();

            GUI.contentColor = oldTextColor;
            GUI.backgroundColor = oldBgColor;
        }

        private void DrawPerlinProperties()
        {
            if (selected._resolution.x < 1)
                selected._resolution = new Vector2Int(1, selected._resolution.y);

            if (selected._resolution.y < 1)
                selected._resolution = new Vector2Int(selected._resolution.x, 1);

            if (selected._width.x <= 0f)
                selected._width = new Vector2(1f, selected._width.y);

            if (selected._width.y <= 0f)
                selected._width = new Vector2(selected._width.x, 1f);

            if (selected._maxHeight < selected._minHeight)
                selected._maxHeight = selected._minHeight;

            if (selected._noiseDensity < 0f)
                selected._noiseDensity = 1f;

            selected._resolution = EditorGUILayout.Vector2IntField("Resolution XY", selected._resolution);
            selected._width = EditorGUILayout.Vector2Field("Width XY", selected._width);

            EditorGUILayout.Space();
            selected._minHeight = EditorGUILayout.FloatField("Min Height Limit", selected._minHeight);
            selected._maxHeight = EditorGUILayout.FloatField("Max Height Limit", selected._maxHeight);
            selected._noiseDensity = EditorGUILayout.FloatField("Noise Density", selected._noiseDensity);

            EditorGUILayout.Space();
            selected._randomize = EditorGUILayout.Toggle("Randomize", selected._randomize);
            selected._addRandomSmallNoises = EditorGUILayout.Toggle("Add Random Small Noises", selected._addRandomSmallNoises);

            if (selected._addRandomSmallNoises)
            {
                selected._smallNoiseRange =
                    EditorGUILayout.Slider("Small Noise Range", selected._smallNoiseRange, 0.01f, 1.0f);
            }
        }

        private void DrawPerlinButtons()
        {
            GUIStyle btnWhiteFont = new GUIStyle(GUI.skin.button);
            btnWhiteFont.normal.textColor = Color.white;

            EditorGUILayout.Space();
            GUI.backgroundColor = Color.blue;
            if (GUILayout.Button("Generate Mesh", btnWhiteFont))
            {
                selected.GenerateMesh();
            }

            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();

            GUI.backgroundColor = selected._showVertexGizmo ? Color.green : Color.black;
            if (GUILayout.Button("Show Vertex", btnWhiteFont))
            {
                selected._showVertexGizmo = !selected._showVertexGizmo;
                FocusToSceneView();
            }

            GUI.backgroundColor = selected._showEdgeGizmo ? Color.green : Color.black;
            if (GUILayout.Button("Show Edge", btnWhiteFont))
            {
                selected._showEdgeGizmo = !selected._showEdgeGizmo;
                FocusToSceneView();
            }
            GUILayout.EndHorizontal();
        }
    }
}
#endif