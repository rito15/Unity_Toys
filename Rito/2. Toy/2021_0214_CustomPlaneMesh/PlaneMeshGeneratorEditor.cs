#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Rito
{
    // 날짜 : 2021-02-14 03:03
    // 작성자 : Rito
    [CustomEditor(typeof(PlaneMeshGenerator))]
    public class PlaneMeshGeneratorEditor : UnityEditor.Editor
    {
        private PlaneMeshGenerator pmg;

        private void OnEnable()
        {
            pmg = target as PlaneMeshGenerator;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            if (GUILayout.Button("Generate"))
            {
                pmg.Generate();
            }
        }
    }
}
#endif