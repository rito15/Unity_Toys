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

        private object _localRotationGUI; // 사용 이유 : 안쓰면 Euler.x 짐벌락 걸림
        private System.Reflection.MethodInfo _trguiOnEnableMethod;
        private System.Reflection.MethodInfo _trguiRotationFieldMethod;

        private void OnEnable()
        {
            transform = target as Transform;

            texturePath = TransformEditorHelper.folderPath + @"\Refresh.png";
            refreshTexture = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;

            // 치트키 : 기존 TransformEditor로부터 RotationField 빌려쓰기
            if (_localRotationGUI == null)
            {
                var bunch = TransformEditorHelper.GetTransformRotationGUI();
                _localRotationGUI = bunch.rotationGUI;
                _trguiOnEnableMethod = bunch.OnEnable;
                _trguiRotationFieldMethod = bunch.RotationField;
            }
            _trguiOnEnableMethod.Invoke(_localRotationGUI, new object[] {
                base.serializedObject.FindProperty("m_LocalRotation"), EditorGUIUtility.TrTextContent("Local Rotation")
            });
        }

        public override void OnInspectorGUI()
        {
            Color oldBgColor = GUI.backgroundColor;
            Color oldTxtColor = GUI.contentColor;
            GUI.backgroundColor = Color.green;
            GUI.contentColor = Color.white;
            if (GUILayout.Button("Reset"))
            {
                transform.localPosition = Vector3.zero;
                transform.localScale = Vector3.one;

                TransformUtils.SetInspectorRotation(transform, Vector3.zero);
            }
            GUI.backgroundColor = oldBgColor;
            GUI.contentColor = oldTxtColor;

            base.serializedObject.Update();

            DrawDefaultTransformInspector();

            EditorGUILayout.Space();
            if (advancedFoldOut = EditorGUILayout.Foldout(advancedFoldOut, "Advanced"))
            {
                DrawGlobalTransformValues();
            }

            base.serializedObject.ApplyModifiedProperties();
        }

        private void DrawDefaultTransformInspector()
        {
            // ==================================== Local Position =========================================
            EditorGUILayout.BeginHorizontal();

            Undo.RecordObject(transform, "Transform Local Position Changed");
            transform.localPosition = EditorGUILayout.Vector3Field("Local Position", transform.localPosition);
            transform.localPosition = Vector3E4Clamp(transform.localPosition);

            Color oldBgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button(refreshTexture, GUILayout.Width(20f), GUILayout.Height(18f)))
            {
                transform.localPosition = Vector3.zero;
            }
            GUI.backgroundColor = oldBgColor;
            EditorGUILayout.EndHorizontal();

            // ==================================== Local Rotation =========================================
            EditorGUILayout.BeginHorizontal();

            Undo.RecordObject(transform, "Transform Local Rotation Changed");

            _trguiRotationFieldMethod.Invoke(_localRotationGUI, new object[] { });

            // 0~360 값 벗어나면 제한
            Vector3 exposedLocalEulerAngle = TransformUtils.GetInspectorRotation(transform);
            if (exposedLocalEulerAngle.x < 0f || exposedLocalEulerAngle.y < 0f || exposedLocalEulerAngle.z < 0f ||
                exposedLocalEulerAngle.x > 360f || exposedLocalEulerAngle.y > 360f || exposedLocalEulerAngle.z > 360f)
            {
                TransformUtils.SetInspectorRotation(transform, transform.localEulerAngles);
            }

            oldBgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button(refreshTexture, GUILayout.Width(20f), GUILayout.Height(18f)))
            {
                TransformUtils.SetInspectorRotation(transform, Vector3.zero);
            }
            GUI.backgroundColor = oldBgColor;

            EditorGUILayout.EndHorizontal();

            // ==================================== Local Scale =========================================
            EditorGUILayout.BeginHorizontal();

            Undo.RecordObject(transform, "Transform Local Scale Changed");
            transform.localScale = EditorGUILayout.Vector3Field("Local Scale", transform.localScale);
            transform.localScale = Vector3E4Clamp(transform.localScale);

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
                transform.position = Vector3.zero;
                transform.eulerAngles = Vector3.zero;
                ChangeGlobalScale(Vector3.one);
            }
            GUI.backgroundColor = oldBgColor;
            GUI.contentColor = oldTxtColor;


            // ==================================== Global Position =========================================
            EditorGUILayout.BeginHorizontal();

            Undo.RecordObject(transform, "Transform Global Position Changed");
            transform.position = EditorGUILayout.Vector3Field("Global Position", transform.position);

            oldBgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.blue;
            if (GUILayout.Button(refreshTexture, GUILayout.Width(20f), GUILayout.Height(18f)))
            {
                transform.position = Vector3.zero;
            }
            GUI.backgroundColor = oldBgColor;
            EditorGUILayout.EndHorizontal();

            // ==================================== Global Rotation =========================================
            EditorGUILayout.BeginHorizontal();

            Undo.RecordObject(transform, "Transform Global Rotation Changed");
            var globalRot = EditorGUILayout.Vector3Field("Global Rotation", transform.eulerAngles);
            transform.eulerAngles = Vector3E4Clamp(globalRot);

            oldBgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.blue;
            if (GUILayout.Button(refreshTexture, GUILayout.Width(20f), GUILayout.Height(18f)))
            {
                //transform.rotation = default; // 안됨. 아래처럼 해야 함
                transform.eulerAngles = Vector3.zero;
            }
            GUI.backgroundColor = oldBgColor;
            EditorGUILayout.EndHorizontal();

            // ==================================== Global Scale =========================================
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

        private Vector3 Vector3E4Clamp(Vector3 vector)
        {
            vector.x = Mathf.Round(vector.x * 10000f) * 0.0001f;
            vector.y = Mathf.Round(vector.y * 10000f) * 0.0001f;
            vector.z = Mathf.Round(vector.z * 10000f) * 0.0001f;
            return vector;
        }
    }
}
#endif