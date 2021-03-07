#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

#pragma warning disable CS0618 // Obsolete

// 날짜 : 2021-03-08 AM 1:12:05
// 작성자 : Rito

namespace Rito
{
    /// <summary> 주기적으로 현재 씬 자동 저장 </summary>
    [InitializeOnLoad]
    public static class SceneAutoSaver
    {
        public static bool Activated { get; set; } = false;
        public static double SaveCycle
        {
            get => _saveCycle;
            set
            {
                if(value < 10f) value = 10f;
                _saveCycle = value;
            }
        }
        public static DateTime LastSavedTimeForLog { get; private set; } // 최근 저장 시간(보여주기용)
        public static double NextSaveRemaining { get; private set; }

        private static double _saveCycle = 10f;
        private static DateTime _lastSavedTime; // 최근 저장 시간
        
        // 정적 생성자 : 에디터 Update 이벤트에 핸들러 등록
        static SceneAutoSaver()
        {
            var handlers = EditorApplication.update.GetInvocationList();

            bool hasAlready = false;
            foreach (var handler in handlers)
            {
                if(handler.Method.Name == nameof(UpdateAutoSave))
                    hasAlready = true;
            }

            if(!hasAlready)
                EditorApplication.update += UpdateAutoSave;

            _lastSavedTime = LastSavedTimeForLog = DateTime.Now;
        }
        
        // 시간을 체크하여 자동 저장
        private static void UpdateAutoSave()
        {
            if (!Activated || EditorApplication.isPlaying) return;
            if (!EditorApplication.isSceneDirty)
            {
                _lastSavedTime = DateTime.Now;
                NextSaveRemaining = _saveCycle;
                return;
            }

            // 시간 계산
            DateTime dt = DateTime.Now;
            double diff = dt.Subtract(_lastSavedTime).TotalSeconds;

            NextSaveRemaining = SaveCycle - diff;
            if(NextSaveRemaining < 0f) NextSaveRemaining = 0f;

            // 정해진 시간 경과 시 저장 및 최근 저장 시간 갱신
            if (diff > SaveCycle)
            {
                //if(EditorApplication.isSceneDirty)
                    EditorSceneManager.SaveOpenScenes();

                _lastSavedTime = LastSavedTimeForLog = DateTime.Now;
            }
        }
    }
}

#endif