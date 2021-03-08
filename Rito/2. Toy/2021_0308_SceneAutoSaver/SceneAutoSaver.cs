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
        private const string Prefix = "SAS_";

        public static bool Activated { get; set; }
        public static bool ShowLog { get; set; }
        public static double SaveCycle
        {
            get => _saveCycle;
            set
            {
                if(value < 5f) value = 5f;
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

            LoadOptions();
        }

        public static void SaveOptions()
        {
            EditorPrefs.SetBool(Prefix + nameof(Activated), Activated);
            EditorPrefs.SetBool(Prefix + nameof(ShowLog), ShowLog);
            EditorPrefs.SetFloat(Prefix + nameof(SaveCycle), (float)SaveCycle);
        }

        private static void LoadOptions()
        {
            Activated = EditorPrefs.GetBool(Prefix + nameof(Activated), true);
            ShowLog   = EditorPrefs.GetBool(Prefix + nameof(ShowLog), true);
            SaveCycle = EditorPrefs.GetFloat(Prefix + nameof(SaveCycle), 10f);

            // 소수점 두자리 컷
            SaveCycle = Math.Floor(SaveCycle * 100.0) * 0.01;
        }
        
        // 시간을 체크하여 자동 저장
        private static void UpdateAutoSave()
        {
            DateTime dtNow = DateTime.Now;

            if (!Activated || EditorApplication.isPlaying || !EditorApplication.isSceneDirty)
            {
                _lastSavedTime = dtNow;
                NextSaveRemaining = _saveCycle;
                return;
            }

            // 시간 계산
            double diff = dtNow.Subtract(_lastSavedTime).TotalSeconds;

            NextSaveRemaining = SaveCycle - diff;
            if(NextSaveRemaining < 0f) NextSaveRemaining = 0f;

            // 정해진 시간 경과 시 저장 및 최근 저장 시간 갱신
            if (diff > SaveCycle)
            {
                //if(EditorApplication.isSceneDirty)
                EditorSceneManager.SaveOpenScenes();
                _lastSavedTime = LastSavedTimeForLog = dtNow;

                if (ShowLog)
                {
                    string dateStr = dtNow.ToString("yyyy-MM-dd  hh:mm:ss");
                    UnityEngine.Debug.Log($"[Auto Save] {dateStr}");
                }
            }
        }
    }
}

#endif