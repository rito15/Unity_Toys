#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Rito
{
    [InitializeOnLoad]
    public class PlayModeStateChangeHandler : ScriptableObject
    {
        public static event Action OnExitEditMode;
        public static event Action OnEnterPlayMode;
        public static event Action OnExitPlayMode;
        public static event Action OnEnterEditMode;

        static PlayModeStateChangeHandler()
        {
            //EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
        }
        private static void OnPlaymodeStateChanged(PlayModeStateChange stateChange)
        {
            switch (stateChange)
            {
                // EditMode -> PlayMode 1
                case PlayModeStateChange.ExitingEditMode:
                    OnExitEditMode?.Invoke();
                    break;

                // EditMode -> PlayMode 2
                // ��� Start()�� ����ǰ�, ������ 2��° Update() ���Ŀ� ����
                case PlayModeStateChange.EnteredPlayMode:
                    OnEnterPlayMode?.Invoke();
                    break;

                // PlayMode -> EditMode 1
                case PlayModeStateChange.ExitingPlayMode:
                    OnExitPlayMode?.Invoke();
                    break;

                // PlayMode -> EditMode 2
                case PlayModeStateChange.EnteredEditMode:
                    OnEnterEditMode?.Invoke();
                    break;
            }
        }
    }
}

#endif