using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-04-19 PM 7:31:51

namespace Rito.DragAndDropRecorder
{
    /// <summary> 기록기 </summary>
    [DisallowMultipleComponent]
    public class DDRecorder : MonoBehaviour
    {
        /***********************************************************************
        *                               Singleton
        ***********************************************************************/
        #region .
        private static DDRecorder _instance;
        public static DDRecorder Instance
        {
            get
            {
                if(_instance == null)
                    CreateInstance();

                return _instance;
            }
        }

        private static void CreateInstance()
        {
            GameObject go = new GameObject("Drag and Drop Recorder Instance");
            _instance = go.AddComponent<DDRecorder>();
        }

        private void CheckInstance()
        {
            if (_instance != null)
            {
                if (_instance != this)
                    Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        private void Awake()
        {
            CheckInstance();
        }

        #endregion

        /// <summary> 최대 기록 제한 시간 </summary>
        private const float MaxRecordLimit = 5f;

        [Tooltip("기록 간격")]
        public float _recordCycle = 0.1f;

        [Tooltip("반복 재생 대상")]
        public Transform _replayTarget;

        // 기록 대상
        private DDTarget _recordTarget;

        // 기록 리스트
        private List<RecordData> _recordList = new List<RecordData>();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopCoroutine(nameof(ReplayRoutine));
                StartCoroutine(nameof(ReplayRoutine));
            }
        }

        public void BeginRecord()
        {
            Debug.Log("Begin Record");

            _recordList.Clear();
            StartCoroutine(nameof(RecordRoutine));
        }

        public void EndRecord()
        {
            Debug.Log("End Record");

            StopCoroutine(nameof(RecordRoutine));
        }

        public void SetRecordTarget(DDTarget target)
        {
            _recordTarget = target;
        }

        private IEnumerator RecordRoutine()
        {
            WaitForSeconds wfs = new WaitForSeconds(_recordCycle);

            float f = 0f;
            for (; f < MaxRecordLimit; f += _recordCycle)
            {
                _recordList.Add(new RecordData(f, _recordTarget.Position));

                yield return wfs;
            }
        }

        /// <summary> 기록된 데이터에 따라 리플레이 타겟 위치 이동 </summary>
        private IEnumerator ReplayRoutine()
        {
            if (_replayTarget == null) yield break;
            if (_recordList.Count == 0) yield break;

            int len = _recordList.Count;
            int curIndex = 0;

            float f = 0f;
            float fEnd = _recordList[len - 1].elapsedTime;

            RecordData curData = _recordList[0];
            RecordData nextData = _recordList[1];

            float gap = nextData.elapsedTime - curData.elapsedTime;

            for (; f < fEnd; f += Time.deltaTime)
            {
                if (f >= nextData.elapsedTime)
                {
                    curIndex++;
                    if (curIndex >= len - 1)
                        yield break;

                    curData = _recordList[curIndex];
                    nextData = _recordList[curIndex + 1];
                }

                float t = (f - curData.elapsedTime) / gap;
                _replayTarget.position = Vector3.Lerp(curData.position, nextData.position, t);

                yield return null;
            }
        }
    }
}