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
        public PerlinNoiseMeshGenerator pn;

        private void OnEnable()
        {
            pn = AssetDatabase.Contains(target) ? null : (PerlinNoiseMeshGenerator)target;
        }

        public override void OnInspectorGUI()
        {
            if (pn == null)
                return;
            Color oldTextColor = GUI.contentColor;
            Color oldBgColor = GUI.backgroundColor;

            EditorGUILayout.Space();

            DrawPerlinProperties();
            DrawPerlinButtons();

            GUI.contentColor = oldTextColor;
            GUI.backgroundColor = oldBgColor;
        }

        protected void DrawPerlinProperties()
        {
            if (pn._resolution.x < 1)
                pn._resolution = new Vector2Int(1, pn._resolution.y);

            if (pn._resolution.y < 1)
                pn._resolution = new Vector2Int(pn._resolution.x, 1);

            if (pn._width.x <= 0f)
                pn._width = new Vector2(1f, pn._width.y);

            if (pn._width.y <= 0f)
                pn._width = new Vector2(pn._width.x, 1f);

            if (pn._maxHeight < pn._minHeight)
                pn._maxHeight = pn._minHeight;

            if (pn._noiseDensity < 0f)
                pn._noiseDensity = 1f;

            pn._resolution = EditorGUILayout.Vector2IntField("Resolution XY", pn._resolution);
            pn._width = EditorGUILayout.Vector2Field("Width XY", pn._width);

            EditorGUILayout.Space();
            pn._minHeight = EditorGUILayout.FloatField("Min Height Limit", pn._minHeight);
            pn._maxHeight = EditorGUILayout.FloatField("Max Height Limit", pn._maxHeight);
            pn._noiseDensity = EditorGUILayout.FloatField("Noise Density", pn._noiseDensity);

            EditorGUILayout.Space();
            pn._randomSeed = EditorGUILayout.IntField("Random Seed", pn._randomSeed);

            EditorGUILayout.Space();
            pn._addRandomSmallNoises = EditorGUILayout.Toggle("Add Random Small Noises", pn._addRandomSmallNoises);

            if (pn._addRandomSmallNoises)
            {
                pn._randomSmallSeed = EditorGUILayout.IntField("Small Random Seed", pn._randomSmallSeed);
                pn._smallNoiseRange =
                    EditorGUILayout.Slider("Small Noise Range", pn._smallNoiseRange, 0.01f, 1.0f);
            }
        }

        protected void DrawPerlinButtons()
        {
            GUIStyle btnWhiteFont = new GUIStyle(GUI.skin.button);
            btnWhiteFont.normal.textColor = Color.white;

            EditorGUILayout.Space();
            GUI.backgroundColor = Color.blue;
            if (GUILayout.Button("Generate Mesh", btnWhiteFont))
            {
                pn.GenerateMesh();
            }

            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();

            GUI.backgroundColor = pn._showVertexGizmo ? Color.green : Color.black;
            if (GUILayout.Button("Show Vertex", btnWhiteFont))
            {
                pn._showVertexGizmo = !pn._showVertexGizmo;
                FocusOnSceneView();
            }

            GUI.backgroundColor = pn._showEdgeGizmo ? Color.green : Color.black;
            if (GUILayout.Button("Show Edge", btnWhiteFont))
            {
                pn._showEdgeGizmo = !pn._showEdgeGizmo;
                FocusOnSceneView();
            }
            GUILayout.EndHorizontal();
        }
    }
}
#endif