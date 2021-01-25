using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-25 PM 2:36:18
// 작성자 : Rito
namespace Rito
{
    public abstract class SingletonMonoBehavior<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region Public Static Properties

        /// <summary> 싱글톤 인스턴스 Getter </summary>
        public static T Instance
        {
            get
            {
                if (_isBreathing == false)    // 체크 : 씬이 올바르게 활성화 되었는지 여부
                    return null;

                // Thread-safe
                lock (_lock)
                {
                    if (_instance == null)    // 체크 : 인스턴스가 없는 경우
                        CheckExsistence();

                    return _instance;
                }
            }
        }

        /// <summary> 싱글톤 인스턴스의 또다른 이름 </summary>
        public static T I => Instance;

        /// <summary>
        /// 싱글톤 게임오브젝트의 참조
        /// </summary>
        public static GameObject ContainerObject
        {
            get
            {
                if (_containerObject == null)
                    CreateContainerObject();

                return _containerObject;
            }
        }

        #endregion // ==========================================================

        #region Private Static Variables

        /// <summary> 싱글톤 인스턴스 </summary>
        private static T _instance;
        private static GameObject _containerObject;

        private static object _lock = new object();

        // Check Validation(Scene)
        public static bool _isBreathing = true;

        #endregion // ==========================================================

        #region Private Static Methods

        // (정적) 싱글톤 인스턴스 존재 여부 확인 (체크 2)
        private static void CheckExsistence()
        {
            // 싱글톤 검색
            _instance = FindObjectOfType<T>();

            // 인스턴스 가진 오브젝트가 존재하지 않을 경우, 빈 오브젝트를 임의로 생성하여 인스턴스 할당
            if (_instance == null)
            {
                // 게임 오브젝트에 클래스 컴포넌트 추가 후 인스턴스 할당
                _instance = ContainerObject.GetComponent<T>();
            }
        }

        // (정적) 싱글톤 컴포넌트를 담을 게임 오브젝트 생성
        private static void CreateContainerObject()
        {
            // null이 아니면 Do Nothing
            if (_containerObject != null) return;

            // 부모 오브젝트 "Singleton Objects" 찾기 or 생성
            GameObject parentContainer = GameObject.Find("Singleton Objects");

            if (parentContainer == null)
                parentContainer = new GameObject("Singleton Objects");

            // 빈 게임 오브젝트 생성
            _containerObject = new GameObject($"[Singleton] {typeof(T)}");

            // 부모 오브젝트에 넣어주기
            _containerObject.transform.SetParent(parentContainer.transform);

            // 인스턴스가 없던 경우, 새로 생성
            if (_instance == null)
                _instance = ContainerObject.AddComponent<T>();
        }

        #endregion // ==========================================================

        #region Private Methods

        // (동적) 싱글톤 스크립트를 미리 오브젝트에 담아 사용하는 경우를 위한 로직 (Awake()에서 호출)
        private void CheckInstance()
        {
            // 싱글톤 인스턴스가 미리 존재하지 않았을 경우, 본인으로 초기화
            if (_instance == null)
            {
                Debug.Log($"싱글톤 생성 : {typeof(T).ToString()}" +
                    $" | 게임 오브젝트 : {name}");

                // 싱글톤 컴포넌트 초기화
                _instance = this as T;

                // 싱글톤 컴포넌트를 담고 있는 게임오브젝트로 초기화
                _containerObject = gameObject;
            }

            // 싱글톤 인스턴스가 존재하는데, 본인이 아닐 경우, 스스로(컴포넌트)를 파괴
            if (_instance != null && _instance != this)
            {
                Debug.Log($"이미 {typeof(T).ToString()} 싱글톤이 존재하므로 오브젝트를 파괴합니다.");
                Destroy(this);

                // 만약 게임 오브젝트에 컴포넌트가 자신만 있었다면, 게임 오브젝트도 파괴
                var components = gameObject.GetComponents<Component>();

                if (components.Length <= 2)
                    Destroy(gameObject);
            }
        }

        #endregion // ==========================================================

        protected virtual void Awake()
        {
            CheckInstance();
        }
    }
}