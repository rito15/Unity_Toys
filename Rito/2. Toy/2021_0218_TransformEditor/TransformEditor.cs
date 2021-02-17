#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

// 날짜 : 2021-02-17 02:06
// 작성자 : Rito

namespace Rito
{
    [CustomEditor(typeof(Transform))]
    public class TransformEditor : UnityEditor.Editor
    {
        private Transform transform;
        private static bool advancedFoldOut;

        private Texture refreshTexture;
        private string texturePath;

        private void OnEnable()
        {
            transform = target as Transform;

            texturePath = TransformEditorHelper.folderPath + @"\Refresh.png";
            refreshTexture = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;
        }

        public override void OnInspectorGUI()
        {
            Color oldBgColor = GUI.backgroundColor;
            Color oldTxtColor = GUI.contentColor;
            GUI.backgroundColor = Color.green;
            GUI.contentColor = Color.white;
            if (GUILayout.Button("Reset"))
            {
                transform.localPosition = default;
                transform.localRotation = default;
                transform.localScale = Vector3.one;
            }
            GUI.backgroundColor = oldBgColor;
            GUI.contentColor = oldTxtColor;

            DrawBasicTransformInspector();

            EditorGUILayout.Space();
            if (advancedFoldOut = EditorGUILayout.Foldout(advancedFoldOut, "Advanced"))
            {
                DrawGlobalTransformValues();
            }
        }

        private void DrawBasicTransformInspector()
        {
            EditorGUILayout.BeginHorizontal();

            Undo.RecordObject(transform, "Transform Local Position Changed");
            transform.localPosition = EditorGUILayout.Vector3Field("Local Position", transform.localPosition);

            Color oldBgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button(refreshTexture, GUILayout.Width(20f), GUILayout.Height(18f)))
            {
                transform.localPosition = default;
            }
            GUI.backgroundColor = oldBgColor;
            EditorGUILayout.EndHorizontal();

            //=============================================================================================

            EditorGUILayout.BeginHorizontal();

            Undo.RecordObject(transform, "Transform Local Rotation Changed");
            var localRot = EditorGUILayout.Vector3Field("Local Rotation", transform.localEulerAngles);
            transform.localEulerAngles = localRot;
            //transform.localEulerAngles = new Vector3(Mathf.Round(localRot.x * 100f) * 0.01f, Mathf.Round(localRot.y * 100f) * 0.01f, Mathf.Round(localRot.z * 100f) * 0.01f);

            oldBgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button(refreshTexture, GUILayout.Width(20f), GUILayout.Height(18f)))
            {
                transform.localRotation = default;
            }
            GUI.backgroundColor = oldBgColor;

            EditorGUILayout.EndHorizontal();
            //=============================================================================================

            EditorGUILayout.BeginHorizontal();

            Undo.RecordObject(transform, "Transform Local Scale Changed");
            transform.localScale = EditorGUILayout.Vector3Field("Local Scale", transform.localScale);

            oldBgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button(refreshTexture, GUILayout.Width(20f), GUILayout.Height(18f)))
            {
                transform.localScale = Vector3.one;
            }
            GUI.backgroundColor = oldBgColor;
            EditorGUILayout.EndHorizontal();
        }

        private void DrawGlobalTransformValues()
        {
            Color oldBgColor = GUI.backgroundColor;
            Color oldTxtColor = GUI.contentColor;
            GUI.backgroundColor = Color.blue;
            GUI.contentColor = Color.white;
            if (GUILayout.Button("Reset"))
            {
                transform.position = default;
                transform.rotation = default;
                ChangeGlobalScale(Vector3.one);
            }
            GUI.backgroundColor = oldBgColor;
            GUI.contentColor = oldTxtColor;


            //=============================================================================================

            EditorGUILayout.BeginHorizontal();

            Undo.RecordObject(transform, "Transform Global Position Changed");
            transform.position = EditorGUILayout.Vector3Field("Global Position", transform.position);

            oldBgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.blue;
            if (GUILayout.Button(refreshTexture, GUILayout.Width(20f), GUILayout.Height(18f)))
            {
                transform.position = default;
            }
            GUI.backgroundColor = oldBgColor;
            EditorGUILayout.EndHorizontal();

            //=============================================================================================

            EditorGUILayout.BeginHorizontal();

            Undo.RecordObject(transform, "Transform Global Rotation Changed");
            var globalRot = EditorGUILayout.Vector3Field("Global Rotation", transform.eulerAngles);
            //transform.eulerAngles = new Vector3(Mathf.Round(globalRot.x * 100f) * 0.01f, Mathf.Round(globalRot.y * 100f) * 0.01f, Mathf.Round(globalRot.z * 100f) * 0.01f);

            oldBgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.blue;
            if (GUILayout.Button(refreshTexture, GUILayout.Width(20f), GUILayout.Height(18f)))
            {
                transform.rotation = default;
            }
            GUI.backgroundColor = oldBgColor;
            EditorGUILayout.EndHorizontal();

            //=============================================================================================

            EditorGUILayout.BeginHorizontal();

            Undo.RecordObject(transform, "Transform Global Scale Changed");
            Vector3 changedGlobalScale = EditorGUILayout.Vector3Field("Global Scale", transform.lossyScale);
            ChangeGlobalScale(changedGlobalScale);

            oldBgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.blue;
            if (GUILayout.Button(refreshTexture, GUILayout.Width(20f), GUILayout.Height(18f)))
            {
                ChangeGlobalScale(Vector3.one);
            }
            GUI.backgroundColor = oldBgColor;
            EditorGUILayout.EndHorizontal();
        }

        System.Collections.Generic.Queue<Vector3> _scaleHierarchy = new System.Collections.Generic.Queue<Vector3>();
        private void ChangeGlobalScale(Vector3 globalScale)
        {
            _scaleHierarchy.Clear();
            Transform parentTr = transform.parent;
            while (parentTr != null)
            {
                _scaleHierarchy.Enqueue(parentTr.localScale);
                parentTr = parentTr.parent;
            }

            while (_scaleHierarchy.Count > 0)
            {
                var current = _scaleHierarchy.Dequeue();
                float x = globalScale.x / current.x;
                float y = globalScale.y / current.y;
                float z = globalScale.z / current.z;
                globalScale.Set(x, y, z);
            }

            transform.localScale = globalScale;
        }
    }
}
#endif