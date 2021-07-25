using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 날짜 : 2021-07-26 AM 2:58:16
// 작성자 : Rito

namespace Rito.Tests
{
    public class Test_SingletonCaller : MonoBehaviour
    {
        private void Awake()
        {
            _ = Test_SingletonA.I;
            _ = Test_SingletonA.I;
            _ = Test_SingletonA.I;
            _ = Test_SingletonA.I;
            _ = Test_SingletonB.I;
            _ = Test_SingletonB.I;
            _ = Test_SingletonB.I;
            _ = Test_SingletonB.I;

            _ = SINGLETON_EXAMPLE.I;
            _ = SINGLETON_EXAMPLE.I;
            _ = SINGLETON_EXAMPLE.I;
            _ = SINGLETON_EXAMPLE.I;
        }
    }
}