#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

// 날짜 : 2021-03-05 PM 9:38:46
// 작성자 : Rito

// 기능
//  - 임포트되는 모델의 트랜스폼을 자동 리셋한다.
//  - 피벗을 모델의 중심 하단 좌표로 위치시킨다.

// 옵션
//  - [Window] - [Rito] - [Model Pivot Resetter] - [Activated]를 통해 동작 여부를 결정할 수 있다.
//  - [Window] - [Rito] - [Model Pivot Resetter] - [Show Dialog]를 체크할 경우,
//    모델을 임포트할 때마다 기능 적용 여부를 다이얼로그를 통해 선택할 수 있다.

namespace Rito
{
    public class ModelPivotResetter : AssetPostprocessor
    {
        private void OnPostprocessModel(GameObject go)
        {
            if(!Activated) return;

            if(!ShowDialog)
                ResetModelPivot(go);

            else if (EditorUtility.DisplayDialog("Model Pivot Resetter", $"Reset Pivot of {go.name}", "Yes", "No"))
                ResetModelPivot(go);
        }

        private void ResetModelPivot(GameObject go)
        {
            var meshes = go.GetComponentsInChildren<MeshFilter>();

            foreach (var meshFilter in meshes)
            {
                Mesh m = meshFilter.sharedMesh;
                Vector3[] vertices = m.vertices;

                // 1. 로컬 트랜스폼 초기화하면서 정점 돌려놓기
                for (int i = 0; i < m.vertexCount; i++)
                {
                    vertices[i] = go.transform.TransformPoint(m.vertices[i]);
                }

                go.transform.localRotation = Quaternion.identity;
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;

                // 2. 피벗을 모델 중심 하단으로 변경
                Vector3 modelToPivotDist = -GetBottomCenterPosition(vertices);

                for (int i = 0; i < m.vertexCount; i++)
                {
                    vertices[i] += modelToPivotDist;
                }

                // 3. 메시에 적용
                m.vertices = vertices;
                m.RecalculateBounds();
                m.RecalculateNormals();

                Debug.Log("Pivot Reset : " + go.name);
            }
        }

        /// <summary> 모델의 XZ 중심, Y 하단 위치 구하기 </summary>
        private Vector3 GetBottomCenterPosition(Vector3[] vertices)
        {
            float minX = float.MaxValue, minZ = float.MaxValue, minY = float.MaxValue;
            float maxX = float.MinValue, maxZ = float.MinValue;

            foreach (var vert in vertices)
            {
                if(minX > vert.x) minX = vert.x;
                if(minZ > vert.z) minZ = vert.z;
                if(minY > vert.y) minY = vert.y;

                if(maxX < vert.x) maxX = vert.x;
                if(maxZ < vert.z) maxZ = vert.z;
            }
            float x = (minX + maxX) * 0.5f;
            float z = (minZ + maxZ) * 0.5f;
            float y = minY;

            return new Vector3(x, y, z);
        }

        /***********************************************************************
        *                               Menu Item
        ***********************************************************************/
        #region .
        // 1. On/Off
        private const string ActivationMenuName = "Window/Rito/Model Pivot Resetter/Activated";
        private const string ActivationSettingName = "ModelPivotResetterActivated";

        public static bool Activated
        {
            get { return EditorPrefs.GetBool(ActivationSettingName, true); }
            set { EditorPrefs.SetBool(ActivationSettingName, value); }
        }

        [MenuItem(ActivationMenuName)]
        private static void ActivationToggle() => Activated = !Activated;

        [MenuItem(ActivationMenuName, true)]
        private static bool ActivationToggleValidate()
        {
            Menu.SetChecked(ActivationMenuName, Activated);
            return true;
        }

        // 2. Show Dialog
        private const string ShowDialogMenuName = "Window/Rito/Model Pivot Resetter/Show Dialog";
        private const string ShowDialogSettingName = "ModelPivotResetterShowDialog";

        public static bool ShowDialog
        {
            get { return EditorPrefs.GetBool(ShowDialogSettingName, true); }
            set { EditorPrefs.SetBool(ShowDialogSettingName, value); }
        }

        [MenuItem(ShowDialogMenuName)]
        private static void ShowDialogToggle() => ShowDialog = !ShowDialog;

        [MenuItem(ShowDialogMenuName, true)]
        private static bool ShowDialogToggleValidate()
        {
            Menu.SetChecked(ShowDialogMenuName, ShowDialog);
            return true;
        }

        #endregion
    }
}

#endif