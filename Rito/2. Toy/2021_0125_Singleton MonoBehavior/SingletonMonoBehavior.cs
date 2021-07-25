#if UNITY_EDITOR
#define DEBUG_ON
#define GATHER_INTO_SAME_PARENT // 하나의 공통 부모 게임오브젝트에 모아놓기
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 작성자 : Rito

// 최초 작성 : 2021-01-25 PM 2:36:18

// 수정 : 2021-07-26 AM 02:35
/* 
 * - 싱글톤 프로퍼티 Getter -> 더블 체크 로킹 제거
 *   - 근거 : 멀티스레딩 환경이면 애초에 Unity API 접근이 예외를 발생시킴
 *   
 * - Scene Breathing 제거
 *   - 근거 : Getter는 최대한 가벼운게 좋음
 */
namespace Rito
{
    [DisallowMultipleComponent]
    public abstract class SingletonMonoBehavior<T> : MonoBehaviour where T : MonoBehaviour
    {
        /***********************************************************************
        *                       Public Static Properties
        ***********************************************************************/
        #region .
        /// <summary> 싱글톤 인스턴스 Getter </summary>
        public static T I
        {
            get
            {
                // 객체 참조 확인
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    // 인스턴스 가진 오브젝트가 존재하지 않을 경우, 빈 오브젝트를 임의로 생성하여 인스턴스 할당
                    if (_instance == null)
                    {
                        // 게임 오브젝트에 클래스 컴포넌트 추가 후 인스턴스 할당
                        _instance = ContainerObject.GetComponent<T>();
                    }
                }
                return _instance;
            }
        }

        /// <summary> 싱글톤 인스턴스 Getter </summary>
        public static T Instance => I;

        /// <summary> 싱글톤 게임오브젝트의 참조 </summary>
        public static GameObject ContainerObject
        {
            get
            {
                if (_containerObject == null)
                    CreateContainerObject();

                return _containerObject;
            }
        }

        #endregion
        /***********************************************************************
        *                       Private Static Variables
        ***********************************************************************/
        #region .

        /// <summary> 싱글톤 인스턴스 </summary>
        private static T _instance;
        private static GameObject _containerObject;

        #endregion
        /***********************************************************************
        *                       Private Static Methods
        ***********************************************************************/
        #region .
        [System.Diagnostics.Conditional("DEBUG_ON")]
        protected static void DebugOnlyLog(string msg)
        {
            Debug.Log(msg);
        }

        /// <summary> 공통 부모 게임오브젝트에 모아주기 </summary>
        [System.Diagnostics.Conditional("GATHER_INTO_SAME_PARENT")]
        protected static void GatherGameObjectIntoSameParent()
        {
            string parentName = "Singleton Objects";

            // 게임오브젝트 "Singleton Objects" 찾기 or 생성
            GameObject parentContainer = GameObject.Find(parentName);
            if (parentContainer == null)
                parentContainer = new GameObject(parentName);

            // 부모 오브젝트에 넣어주기
            _containerObject.transform.SetParent(parentContainer.transform);
        }

        // (정적) 싱글톤 컴포넌트를 담을 게임 오브젝트 생성
        private static void CreateContainerObject()
        {
            // null이 아니면 Do Nothing
            if (_containerObject != null) return;

            // 빈 게임 오브젝트 생성
            _containerObject = new GameObject($"[Singleton] {typeof(T)}");

            // 인스턴스가 없던 경우, 새로 생성
            if (_instance == null)
                _instance = ContainerObject.AddComponent<T>();

            GatherGameObjectIntoSameParent();
        }

        #endregion

        protected virtual void Awake()
        {
            // 싱글톤 인스턴스가 미리 존재하지 않았을 경우, 본인으로 초기화
            if (_instance == null)
            {
                DebugOnlyLog($"싱글톤 생성 : {typeof(T)}, 게임 오브젝트 : {name}");

                // 싱글톤 컴포넌트 초기화
                _instance = this as T;

                // 싱글톤 컴포넌트를 담고 있는 게임오브젝트로 초기화
                _containerObject = gameObject;

                GatherGameObjectIntoSameParent();
            }

            // 싱글톤 인스턴스가 존재하는데, 본인이 아닐 경우, 스스로(컴포넌트)를 파괴
            if (_instance != null && _instance != this)
            {
                DebugOnlyLog($"이미 {typeof(T)} 싱글톤이 존재하므로 오브젝트를 파괴합니다.");

                var components = gameObject.GetComponents<Component>();

                // 만약 게임 오브젝트에 컴포넌트가 자신만 있었다면, 게임 오브젝트도 파괴
                if (components.Length <= 2)
                    Destroy(gameObject);

                // 다른 컴포넌트도 존재하면 자신만 파괴
                else
                    Destroy(this);
            }
        }
    }
}