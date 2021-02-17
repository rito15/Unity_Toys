using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        static void GetFolderPath([System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            folderPath = System.IO.Path.GetDirectoryName(sourceFilePath);
            int rootIndex = folderPath.IndexOf(@"Assets\");
            if (rootIndex > -1)
            {
                folderPath = folderPath.Substring(rootIndex, folderPath.Length - rootIndex);
            }
        }
    }
}