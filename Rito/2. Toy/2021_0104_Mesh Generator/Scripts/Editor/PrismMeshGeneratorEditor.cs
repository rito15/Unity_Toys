#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Rito.MeshGenerator
{
    using static MeshGeneratorEditorHelper;

    [CustomEditor(typeof(PrismMeshGenerator))]
    public class PrismMeshGeneratorEditor : UnityEditor.Editor
    {
        public PrismMeshGenerator selected;

        private void OnEnable()
        {
            selected = AssetDatabase.Contains(target) ? null : (PrismMeshGenerator)target;
        }

        public override void OnInspectorGUI()
        {
            if (selected == null)
                return;

            Color oldTextColor = GUI.contentColor;
            Color oldBgColor = GUI.backgroundColor;

            EditorGUILayout.Space();

            if (selected._topRadius <= 0f) selected._topRadius = 1f;
            if (selected._bottomRadius <= 0f) selected._bottomRadius = 1f;
            if (selected._height <= 0f) selected._height = 1f;
            if (selected._polygonVertex <= 2) selected._polygonVertex = 3;

            selected._topRadius = EditorGUILayout.FloatField("Top Radius", selected._topRadius);
            selected._bottomRadius = EditorGUILayout.FloatField("Bottom Radius", selected._bottomRadius);
            selected._height = EditorGUILayout.FloatField("Height", selected._height);
            selected._polygonVertex = EditorGUILayout.IntField("Vertices", selected._polygonVertex);
            selected._pivotPoint = (PrismMeshGenerator.PivotPoint)EditorGUILayout.EnumPopup("Pivot Point", selected._pivotPoint);

            EditorGUILayout.Space();
            GUI.backgroundColor = Color.blue;
            if (GUILayout.Button("Generate Mesh"))
            {
                selected.GenerateMesh();
            }

            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();

            GUI.backgroundColor = selected._showVertexGizmo ? Color.green : Color.black;
            if (GUILayout.Button("Show Vertex"))
            {
                selected._showVertexGizmo = !selected._showVertexGizmo;
                FocusToSceneView();
            }

            GUI.backgroundColor = selected._showEdgeGizmo ? Color.green : Color.black;
            if (GUILayout.Button("Show Edge"))
            {
                selected._showEdgeGizmo = !selected._showEdgeGizmo;
                FocusToSceneView();
            }
            GUILayout.EndHorizontal();

            GUI.contentColor = oldTextColor;
            GUI.backgroundColor = oldBgColor;
        }
    }
}
#endif