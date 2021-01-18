using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// 날짜 : 2021-01-09 PM 10:31:46
// 작성자 : Rito

namespace Rito.MeshGenerator
{
    public static class MeshGeneratorEditorHelper
    {
        public static void FocusToSceneView()
        {
            if (SceneView.sceneViews.Count > 0)
                (SceneView.sceneViews[0] as SceneView).Focus();
        }
    }
}