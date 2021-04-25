using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rito.Plugins;

// 날짜 : 2021-04-24 PM 7:40:43
// 작성자 : Rito

public class Test_ColorDebug : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Default");
        Debug.Log("<color=white>White</color>");
        Debug.Log("<color=grey>Grey</color>");
        Debug.Log("<color=black>Black</color>");
        Debug.Log("<color=red>Red</color>");
        Debug.Log("<color=green>Green</color>");
        Debug.Log("<color=blue>Blue</color>");
        Debug.Log("<color=yellow>Yellow</color>");
        Debug.Log("<color=cyan>Cyan</color>");
        Debug.Log("<color=brown>Brown</color>");

        Debug.Log("<color=#FAD656>Custom 1</color>");
        Debug.Log("<color=#00FF22>Custom 2</color>");
    }
}