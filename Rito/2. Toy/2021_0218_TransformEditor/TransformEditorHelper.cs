#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;

// 날짜 : 2021-02-18 AM 4:41:01
// 작성자 : Rito

namespace Rito
{
    public static class TransformEditorHelper
    {
        public static string folderPath;
        static TransformEditorHelper()
        {
            GetFolderPath();
        }

        private static void GetFolderPath([System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            folderPath = System.IO.Path.GetDirectoryName(sourceFilePath);
            int rootIndex = folderPath.IndexOf(@"Assets\");
            if (rootIndex > -1)
            {
                folderPath = folderPath.Substring(rootIndex, folderPath.Length - rootIndex);
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
    }
}

#endif