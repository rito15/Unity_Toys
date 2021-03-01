using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

// 2021. 03. 02. 03:02
// 작성자 : Rito

namespace Rito
{
    public class ProjectileShooter : MonoBehaviour
    {
        public enum Direction
        {
            Left, Right, Up, Down, Random
        }

        /***********************************************************************
        *                               Public Fields
        ***********************************************************************/
        #region .
        public Direction _direction = Direction.Random;

        public GameObject _projectilePrefab;
        [Range(1f, 10f)] public float _lifeTime = 5f;
        [Range(1f, 20f)] public float _speed = 10f;
        [Range(1f, 20f)] public float _distanceFromCamera = 10f;

        [Range(0.01f, 1f)]
        private float _clickInterval = 0.1f; // 클릭 허용 간격

        #endregion
        /***********************************************************************
        *                               Private Fields
        ***********************************************************************/
        #region .
        private float _currentClickInterval = 0f;

        #endregion
        /***********************************************************************
        *                               Unity Events
        ***********************************************************************/
        #region .
        private void Start()
        {
            _poolGo = new GameObject("Projectile Pool");
        }
        private void OnEnable()
        {
            if (_projectilePrefab == null)
            {
                Debug.LogError("ProjectileShooter : 투사체 오브젝트를 등록해주세요");
                return;
            }

            _projectilePrefab.SetActive(false);
        }

        private void Update()
        {
            if (_projectilePrefab == null) return;
            if (_currentClickInterval > 0f)
            {
                _currentClickInterval -= Time.deltaTime;
                return;
            }

            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                StartCoroutine(ShootRoutine());
                _currentClickInterval = _clickInterval;
            }
        }

        #endregion
        /***********************************************************************
        *                               Coroutine
        ***********************************************************************/
        #region .
        private IEnumerator ShootRoutine()
        {
            Vector3 moveDir;
            Vector3 worldMove;
            float lifeTime = _lifeTime;

            // 이동 방향 결정
            switch (_direction)
            {
                case Direction.Left: moveDir = Vector3.left; break;
                case Direction.Right: moveDir = Vector3.right; break;
                case Direction.Up: moveDir = Vector3.up; break;
                case Direction.Down: moveDir = Vector3.down; break;
                case Direction.Random:
                default:
                    moveDir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized;
                    break;
            }

            worldMove = Camera.main.transform.TransformDirection(moveDir) * _speed;

            // 투사체 스폰
            GameObject psInstance = Spawn();
            Transform psTransform = psInstance.transform;

            // 투사체 위치 지정
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = _distanceFromCamera;
            psTransform.position = Camera.main.ScreenToWorldPoint(mousePos);

            float t = 0f;
            while (t < lifeTime)
            {
                psTransform.Translate(worldMove * Time.deltaTime, Space.World);

                t += Time.deltaTime;
                yield return null;
            }

            Despawn(psInstance);
        }

        #endregion
        /***********************************************************************
        *                               Pooling
        ***********************************************************************/
        #region .
        private Queue<GameObject> _poolQueue = new Queue<GameObject>();
        private GameObject _poolGo;

        private GameObject Spawn()
        {
            GameObject next;

            if (_poolQueue.Count == 0)
                next = Instantiate(_projectilePrefab);
            else
                next = _poolQueue.Dequeue();

            next.SetActive(true);
            next.transform.SetParent(_poolGo.transform);
            return next;
        }

        private void Despawn(GameObject go)
        {
            go.SetActive(false);
            //go.transform.SetParent(_poolGo.transform);
            _poolQueue.Enqueue(go);
        }

        #endregion
    }
}