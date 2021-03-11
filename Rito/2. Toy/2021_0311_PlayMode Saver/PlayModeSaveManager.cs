#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

// 날짜 : 2021-03-11 PM 8:05:19
// 작성자 : Rito

namespace Rito
{
    [InitializeOnLoad]
    public class PlayModeSaveManager// : ScriptableObject
    {
        /***********************************************************************
        *                               Event Fields
        ***********************************************************************/
        #region .

        public static event Action OnBeforeExitingPlayMode;
        public static event Action OnEnterEditMode;

        #endregion
        /***********************************************************************
        *                               Static Constructor
        ***********************************************************************/
        #region .
        static PlayModeSaveManager()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                // 플레이모드 진입 시 딕셔너리 비우기
                case PlayModeStateChange.EnteredPlayMode:
                    dataForSaveDict.Clear();
                    break;

                // 플레이모드 종료 시 SerializedObject들 업데이트
                case PlayModeStateChange.ExitingPlayMode:

                    // 이벤트 호출
                    OnBeforeExitingPlayMode?.Invoke();

                    // 등록된 SO 업데이트
                    foreach (var so in dataForSaveDict.Values)
                    {
                        so.Update();
                    }
                    break;

                // 에디터모드 진입 시 변경사항 적용
                case PlayModeStateChange.EnteredEditMode:
                    OnEnterEditMode?.Invoke();
                    ApplyAll();
                    break;
            }
        }
        #endregion
        /***********************************************************************
        *                           Save & Restore Component
        ***********************************************************************/
        #region .
        private struct IdPair
        {
            public int objectID;
            public int componentID;
            public IdPair(int objectID, int componentID)
            {
                this.objectID = objectID;
                this.componentID = componentID;
            }
        }

        private static Dictionary<IdPair, SerializedObject> dataForSaveDict
            = new Dictionary<IdPair, SerializedObject>();

        /// <summary> 에디터모드로 돌아왔을 때 복원할 컴포넌트 등록 </summary>
        public static void AddComponentForSave(Component component)
        {
            if(component == null) return;

            // ID 얻기
            var cID = component.GetInstanceID();
            var oID = component.gameObject.GetInstanceID();
            var ids = new IdPair(oID, cID);

            // 컴포넌트로부터 SO 생성
            var so = new SerializedObject(component);

            // 딕셔너리에 추가
            if (dataForSaveDict.ContainsKey(ids))
                dataForSaveDict[ids] = so;
            else
                dataForSaveDict.Add(ids, so);
        }

        private static void ApplyComponentChanges(in IdPair ids)
        {
            var so = dataForSaveDict[ids];
            var targetObj = new SerializedObject(so.targetObject);
            var iter = so.GetIterator();

            // SO에 저장된 값들을 SO의 타겟오브젝트에 복원
            while (iter.NextVisible(true))
            {
                targetObj.CopyFromSerializedProperty(iter);
            }
            targetObj.ApplyModifiedProperties();
            dataForSaveDict.Remove(ids);
        }

        private static void ApplyAll()
        {
            if (dataForSaveDict.Count == 0) return;

            var ids = dataForSaveDict.Keys.ToArray();
            foreach (var id in ids)
            {
                // 플레이모드 종료 시 제거된 게임오브젝트들은 제외
                var go = EditorUtility.InstanceIDToObject(id.objectID) as GameObject;
                if (go == null) continue;

                // 컴포넌트도 마찬가지
                var com = EditorUtility.InstanceIDToObject(id.componentID) as Component;
                if (com == null) continue;

                // 변경사항 적용
                ApplyComponentChanges(id);
            }
        }

        #endregion
        /***********************************************************************
        *                           Save & Restore Component - Context Menu
        ***********************************************************************/
        #region .
        private const int PrioritySave = 400;

        /// <summary> 우클릭 메뉴 - 플레이모드 저장 대상 컴포넌트로 리스트에 추가 </summary>
        [MenuItem("CONTEXT/Component/Save PlayMode Changes", false, PrioritySave)]
        private static void Context_SaveChanges(MenuCommand mc)
        {
            var thisComponent = mc.context as Component;

            var pms = thisComponent.GetComponent<PlayModeSaver>();
            if(pms == null)
                pms = thisComponent.gameObject.AddComponent<PlayModeSaver>();

            pms.AddTargetComponentToList(thisComponent);
        }

        [MenuItem("CONTEXT/Component/Save PlayMode Changes", true, PrioritySave)]
        private static bool ValidateContext_SaveChanges(MenuCommand mc)
        {
            if(PrefabUtility.GetPrefabAssetType(mc.context) != PrefabAssetType.NotAPrefab) return false;
            //if(!Application.IsPlaying(mc.context)) return false;

            var thisComponent = mc.context as Component;
            if(thisComponent is PlayModeSaver) return false; // PMS는 대상에서 제외

            var pms = thisComponent.GetComponent<PlayModeSaver>();
            if (pms != null)
            {
                if(pms.CheckContainedInList(thisComponent)) return false;
            }

            return true;
        }

        /// <summary> 우클릭 메뉴 - 플레이모드 저장 대상 컴포넌트에서 제외 </summary>
        [MenuItem("CONTEXT/Component/Don't Save PlayMode Changes", false, PrioritySave)]
        private static void Context_DontSaveChanges(MenuCommand mc)
        {
            var thisComponent = mc.context as Component;

            var pms = thisComponent.GetComponent<PlayModeSaver>();
            if(pms != null)
                pms.RemoveTargetComponentFromList(thisComponent);
        }

        [MenuItem("CONTEXT/Component/Don't Save PlayMode Changes", true, PrioritySave)]
        private static bool ValidateContext_DontSaveChanges(MenuCommand mc)
        {
            if(PrefabUtility.GetPrefabAssetType(mc.context) != PrefabAssetType.NotAPrefab) return false;
            //if(!Application.IsPlaying(mc.context)) return false;

            var thisComponent = mc.context as Component;
            if (thisComponent is PlayModeSaver) return false; // PMS는 대상에서 제외

            var pms = thisComponent.GetComponent<PlayModeSaver>();

            if(pms == null)
                return false;
            else
            {
                // 대상 리스트에 등록되어 있는 경우에만 true
                if(pms.CheckContainedInList(thisComponent)) return true;
            }

            return false;
        }
        #endregion
        /***********************************************************************
        *                           Save & Restore PMS
        ***********************************************************************/
        #region .
        private struct PMSdata
        {
            public bool activated;
            public List<Component> targetList;
        }

        private static Dictionary<int, PMSdata> pmsDict = new Dictionary<int, PMSdata>();

        /// <summary> PlayModeSaver 컴포넌트 데이터 저장 </summary>
        public static void SavePMS(PlayModeSaver pms)
        {
            if (pms == null) return;

            PMSdata data = new PMSdata
            {
                activated = pms._activated,
                targetList = new List<Component>(pms._targetList)
            };

            int id = pms.GetInstanceID();
            if (pmsDict.ContainsKey(id))
                pmsDict[id] = data;
            else
                pmsDict.Add(id, data);

            Debug.Log("Save");
        }

        /// <summary> PlayModeSaver 컴포넌트의 데이터 복구 </summary>
        public static void RestorePMS(int cid, int gid)
        {
            // PMS가 플레이모드 내에서 제거됐다면, 에디터모드에 돌아와서 정말로 제거해주기
            if (pmsDict.ContainsKey(cid) == false)
            {
                var tmpPms = EditorUtility.InstanceIDToObject(cid) as PlayModeSaver;
                if (tmpPms)
                    UnityEngine.Object.DestroyImmediate(tmpPms);

                return;
            }

            // 게임오브젝트가 플레이모드에서 잠깐 생성된 녀석이면 무시하기
            var go = EditorUtility.InstanceIDToObject(gid) as GameObject;
            if (go == null) return;

            // PMS가 플레이모드에서 잠깐 생성한 녀석이라면, 에디터모드에 돌아왔을때 생성해서 넣어주기
            var pms = EditorUtility.InstanceIDToObject(cid) as PlayModeSaver;
            if (pms == null)
            {
                pms = go.AddComponent<PlayModeSaver>();
            }

            // 리스트 내부를 null이 아닌 컴포넌트들로 필터링
            List<Component> listCopy =
                new List<Component>(pmsDict[cid].targetList.Where(c => c != null));

            pms._activated = pmsDict[cid].activated;
            pms._targetList = listCopy;

            Debug.Log("Restore");
        }

        #endregion
    }
}

#endif