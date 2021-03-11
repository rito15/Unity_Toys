using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Reflection;
using System.Linq;

// 날짜 : 2021-03-11 PM 8:38:20
// 작성자 : Rito

public class Test_Reflection : MonoBehaviour
{
    // 특정 네임스페이스에 있는 모든 클래스 타입 가져오기
    //[UnityEditor.InitializeOnLoadMethod] 
    private static void GetAllClassTypesInNamespace()
    {
        string assName = "UnityEngine, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
        string nsName = "UnityEngine";

        var classTypes =
            AppDomain.CurrentDomain.GetAssemblies()    // 모든 어셈블리 대상
                .Where(ass => ass.FullName == assName) // 특정 어셈블리(exe, dll)로 필터링
                .SelectMany(ass => ass.GetTypes())
                .Where(t => t.IsClass && t.Namespace == nsName); // 특정 네임스페이스로 필터링

        // * 모든 어셈블리가 아니라 현재 어셈블리에서 확인하려면
        var classTypes2 = 
            Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass /*&& t.Namespace == nsName*/);

        foreach (var ct in classTypes2)
        {
            Debug.Log(ct);
        }
    }
}