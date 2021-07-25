
/***********************************************************************
*                           Singleton Options
***********************************************************************/
#region .
#if UNITY_EDITOR

// 1. DebugLog() 출력 여부
#define DEBUG_ON

// 2. 하나의 공통 부모 게임오브젝트에 모아놓기
#define GATHER_INTO_SAME_PARENT
#endif

// 3. DontDestroyOnLoad 설정
#define DONT_DESTROY_ON_LOAD

#endregion//***********************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rito
{
    [DisallowMultipleComponent]
    public class SINGLETON_EXAMPLE : MonoBehaviour
    {
        /***********************************************************************
        *                               Singleton
        ***********************************************************************/
        #region .
        /// <summary> 싱글톤 인스턴스 Getter </summary>
        public static SINGLETON_EXAMPLE I
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<SINGLETON_EXAMPLE>();
                    if (_instance == null) _instance = ContainerObject.GetComponent<SINGLETON_EXAMPLE>();
                }
                return _instance;
            }
        }

        /// <summary> 싱글톤 인스턴스 Getter </summary>
        public static SINGLETON_EXAMPLE Instance => I;
        private static SINGLETON_EXAMPLE _instance;

        /// <summary> 싱글톤 게임오브젝트의 참조 </summary>
        private static GameObject ContainerObject
        {
            get
            {
                if (_containerObject == null)
                {
                    _containerObject = new GameObject($"[Singleton] {nameof(SINGLETON_EXAMPLE)}");
                    if (_instance == null) _instance = ContainerObject.AddComponent<SINGLETON_EXAMPLE>();
                    GatherGameObjectIntoSameParent();
                }

                return _containerObject;
            }
        }
        private static GameObject _containerObject;

        /***********************************************************************
        *                               Debug
        ***********************************************************************/
        [System.Diagnostics.Conditional("DEBUG_ON")]
        private static void DebugLog(string msg)
        {
            Debug.Log(msg);
        }

        /// <summary> 공통 부모 게임오브젝트에 모아주기 </summary>
        [System.Diagnostics.Conditional("GATHER_INTO_SAME_PARENT")]
        private static void GatherGameObjectIntoSameParent()
        {
#if !DONT_DESTROY_ON_LOAD
            string parentName = "Singleton Objects";

            GameObject parentContainer = GameObject.Find(parentName);
            if (parentContainer == null)
                parentContainer = new GameObject(parentName);

            _containerObject.transform.SetParent(parentContainer.transform);
#endif
        }

        /***********************************************************************
        *                               Private
        ***********************************************************************/
        private void CheckOrCreateSingletonInstance()
        {
            // 싱글톤 인스턴스가 미리 존재하지 않았을 경우, 본인으로 초기화
            if (_instance == null)
            {
                DebugLog($"싱글톤 생성 : {nameof(SINGLETON_EXAMPLE)}, 게임 오브젝트 : {name}");

                _instance = this;
                _containerObject = gameObject;
                GatherGameObjectIntoSameParent();
            }

            // 싱글톤 인스턴스가 존재하는데, 본인이 아닐 경우, 스스로(컴포넌트)를 파괴
            if (_instance != null && _instance != this)
            {
                DebugLog($"이미 {nameof(SINGLETON_EXAMPLE)} 싱글톤이 존재하므로 오브젝트를 파괴합니다.");

                var components = gameObject.GetComponents<Component>();
                if (components.Length <= 2) Destroy(gameObject);
                else Destroy(this);
            }
#if DONT_DESTROY_ON_LOAD
            if (_instance == this)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(this);
                DebugLog($"Don't Destroy on Load : {nameof(SINGLETON_EXAMPLE)}");
            }
#endif
        }
        #endregion // Singleton

        private void Awake()
        {
            CheckOrCreateSingletonInstance();
        }
    }
}