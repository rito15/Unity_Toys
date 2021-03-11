using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-03-11 PM 8:05:03
// 작성자 : Rito

namespace Rito
{
    [DisallowMultipleComponent]
    public class PlayModeSaver : MonoBehaviour
    {
#if !UNITY_EDITOR
        private void Awake()
        {
            Destroy(this);
            return;
        }
#else
        public bool _activated = true;
        public List<Component> _targetList = new List<Component>();

        private int cid;
        private int gid;

        private void Awake()
        {
            PlayModeSaveManager.OnBeforeExitingPlayMode += () =>
            {
                // 플레이모드 변경사항 저장할 컴포넌트 목록 전달
                if (_activated && _targetList != null && _targetList.Count > 0)
                {
                    foreach (var com in _targetList)
                    {
                        PlayModeSaveManager.AddComponentForSave(com);
                    }
                }

                // 이 컴포넌트의 변경사항 저장
                PlayModeSaveManager.SavePMS(this);
            };

            cid = this.GetInstanceID();
            gid = gameObject.GetInstanceID();

            // 이 컴포넌트의 변경사항 복원
            PlayModeSaveManager.OnEnterEditMode += () =>
            {
                PlayModeSaveManager.RestorePMS(cid, gid);
            };
        }

        /// <summary> 대상 컴포넌트를 중복되지 않게 리스트에 추가 </summary>
        public void AddTargetComponentToList(Component com)
        {
            if(com == null) return;
            if(_targetList.Contains(com)) return;
            _targetList.Add(com);
        }

        /// <summary> 대상 컴포넌트를 리스트에서 제거 </summary>
        public void RemoveTargetComponentFromList(Component com)
        {
            if(com == null) return;
            if(_targetList.Contains(com))
                _targetList.Remove(com);
        }

        public bool CheckContainedInList(Component com)
        {
            return (com != null && _targetList.Contains(com));
        }
#endif
    }
}