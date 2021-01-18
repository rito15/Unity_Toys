#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Rito.MeshGenerator
{
    using static MeshGeneratorEditorHelper;

    [CustomEditor(typeof(RegularPolygonMeshGenerator))]
    public class RegularPolygonMeshGeneratorEditor : UnityEditor.Editor
    {
        public RegularPolygonMeshGenerator selected;

        private void OnEnable()
        {
            selected = AssetDatabase.Contains(target) ? null : (RegularPolygonMeshGenerator)target;
        }

        public override void OnInspectorGUI()
        {
            if (selected == null)
                return;
            Color oldTextColor = GUI.contentColor;
            Color oldBgColor = GUI.backgroundColor;

            EditorGUILayout.Space();

            if (selected._radius <= 0f) selected._radius = 1f;
            if (selected._polygonVertex <= 2) selected._polygonVertex = 3;

            selected._radius = EditorGUILayout.FloatField("Radius", selected._radius);
            selected._polygonVertex = EditorGUILayout.IntField("Vertices", selected._polygonVertex);

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