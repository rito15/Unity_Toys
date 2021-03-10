#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;

// 날짜 : 2021-02-18 AM 4:41:01
// 작성자 : Rito

namespace Rito
{
    [InitializeOnLoad]
    public static class TransformEditorHelper
    {
        public static string FolderPath { get; private set; }
        static TransformEditorHelper()
        {
            InitFolderPath();

            // Load Adv Foldout Value
            TransformEditor.LoadGlobalFoldOutValue(EditorPrefs.GetBool(GlobalPrefName, false));
        }

        private static void InitFolderPath([System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            FolderPath = System.IO.Path.GetDirectoryName(sourceFilePath);
            int rootIndex = FolderPath.IndexOf(@"Assets\");
            if (rootIndex > -1)
            {
                FolderPath = FolderPath.Substring(rootIndex, FolderPath.Length - rootIndex);
            }
        }

        public static (object rotationGUI, MethodInfo OnEnable, MethodInfo RotationField) GetTransformRotationGUI()
        {
            object target;
            MethodInfo onEnableMethod;
            MethodInfo rotationFieldMethod;

            Type targetType = Type.GetType("UnityEditor.TransformRotationGUI, UnityEditor");
            target = Activator.CreateInstance(targetType);

            onEnableMethod = targetType.GetMethod (
                "OnEnable",
                new Type[] { typeof(UnityEditor.SerializedProperty), typeof(GUIContent) }
            );
            rotationFieldMethod = targetType.GetMethod("RotationField", new Type[] { });

            return (target, onEnableMethod, rotationFieldMethod);
        }
        /***********************************************************************
        *                               EditorPrefs
        ***********************************************************************/
        #region .
        private const string GlobalPrefName = "TE_GlobalFoldOut";

        public static void SaveGlobalFoldOutPref(bool value) => EditorPrefs.SetBool(GlobalPrefName, value);

        #endregion
    }
}

#endif