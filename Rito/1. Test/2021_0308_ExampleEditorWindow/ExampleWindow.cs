#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// EditorWindow는 ScriptableObject를 상속함
public class ExampleWindow : EditorWindow
{
    // 모든 필드는
    //  - 컴파일되면 값 손실(기본값 또는 정적 생성자에서 지정한 값으로 초기화)
    //  - 프로젝트 종료 시에 값 손실
    
    // 값을 항상 유지하려면
    //  - EditorPrefs 이용
    //  - File I/O 이용
    //  - 스크립터블 오브젝트를 싱글턴으로 이용?

    // 정적 필드
    //  - 창이 닫혀도 값 유지
    //  - 플레이모드에 진입할 때 값 손실
    private static bool boolValue;

    // 동적 필드
    //  - 창이 닫히면 값 손실
    //  - 플레이모드 상태가 바뀌어도 값 유지
    private float floatValue;
    private Vector3 vector3Value;

    [MenuItem("Window/Rito/Example Window")]
    private static void Init()
    {
        // 현재 활성화된 윈도우 가져오며, 없으면 새로 생성
        ExampleWindow window = (ExampleWindow)GetWindow(typeof(ExampleWindow));
        window.Show();

        // 윈도우 타이틀 지정
        window.titleContent.text = "W I N D O W";

        // 최소, 최대 크기 지정
        window.minSize = new Vector2(340f, 150f);
        window.maxSize = new Vector2(340f, 200f);
    }

    // 정적 생성자 호출 시기
    // [InitializeOnLoad] 선언한 경우
    //  - 컴파일 시
    //  - 플레이모드 진입 시
    //  - 에디터 윈도우를 열 때
    //
    //  선언하지 않은 경우
    //  - 에디터 윈도우가 열려 있는 상태에서 컴파일 시
    //  - 에디터 윈도우가 열려 있는 상태에서 플레이모드 진입 시
    //  - 에디터 윈도우를 열 때
    static ExampleWindow()
    { 
        Debug.Log("Static Constructor");
    }

    // OnEnable 호출 시기
    //  - 에디터 윈도우를 열 때
    //  - 플레이모드 진입 이전
    private void OnEnable()
    {
        Debug.Log("OnEnable : " + EditorApplication.isPlaying);
    }

    // OnEnable 호출 시기
    //  - 에디터 윈도우를 닫을 때
    //  - 플레이모드 진입 이전
    private void OnDisable()
    {
        Debug.Log("OnDisable : " + EditorApplication.isPlaying);
    }

    private void OnGUI()
    {
        // 굵은 글씨 
        Color originColor = EditorStyles.boldLabel.normal.textColor;
        EditorStyles.boldLabel.normal.textColor = Color.yellow;

        // Header =====================================================================
        GUILayout.Space(10f);
        GUILayout.Label("Header Label", EditorStyles.boldLabel);

        vector3Value = EditorGUILayout.Vector3Field("Vector3", vector3Value);

        // ============================================================================
        GUILayout.Space(10f);
        GUILayout.Label("Horizontal", EditorStyles.boldLabel);

        // Horizontal =================================================================
        GUILayout.BeginVertical();

        boolValue = EditorGUILayout.Toggle("Bool", boolValue);
        floatValue = EditorGUILayout.FloatField("Float", floatValue);

        GUILayout.EndVertical();

        // Horizontal =================================================================
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label("Label Left");
            GUILayout.Label("Label Right");
        }
        // ============================================================================

        EditorStyles.boldLabel.normal.textColor = originColor;
    }
}

#endif