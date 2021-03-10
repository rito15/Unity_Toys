using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Rito;
using Debug = Rito.Debug;

// 날짜 : 2021-03-11 AM 3:52:00
// 작성자 : Rito

public class Test_PlayModeChange : MonoBehaviour
{
    private void Start()
    {
        PlayModeStateChangeHandler.OnExitPlayMode += () => Debug.Log("플레이모드 종료");
        PlayModeStateChangeHandler.OnEnterPlayMode += () => Debug.Log("플레이모드 진입");
        PlayModeStateChangeHandler.OnEnterEditMode += () => Debug.Log("에디터모드 진입");
        PlayModeStateChangeHandler.OnExitEditMode += () => Debug.Log("에디터모드 종료");
    }
}