using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 날짜 : 2021-07-28 AM 2:05:41
// 작성자 : Rito

namespace Rito
{
    /// <summary> 
    /// 무한 횡스크롤 UI
    /// </summary>
    public class InfiniteHorizontalScroll : MonoBehaviour
    {
        /***********************************************************************
        *                           Inspector Fields
        ***********************************************************************/
        #region .
        [SerializeField] private Vector2 centerUISize = new Vector2(200f, 200f);  // 중앙 UI의 크기
        [SerializeField] private Vector2 edgeUISize = new Vector2(100f, 100f);    // 가장 외곽 UI의 크기

        [Range(0f, 100f)]
        [SerializeField] private float spaceWidth = 25f; // 이미지 사이 간격

        [Range(0.01f, 2f)]
        [SerializeField] private float transitionTime = 0.5f; // 전환에 걸리는 시간

        [SerializeField] private bool useImposter = true; // 임포스터 사용 여부

        #endregion
        /***********************************************************************
        *                           Private Fields
        ***********************************************************************/
        #region .
        private const int LEFT = 1;
        private const int RIGHT = -1;

        private List<RectTransform> _targetList = new List<RectTransform>(8);
        private List<RectTransform> _imposterList = new List<RectTransform>(8);
        private int _targetCount;

        private RectTransform _currentImposter;

        [SerializeField]
        private int _currentIndex = 0;

        private bool _isTransiting = false;
        private float _progress; // 진행도 : 0 ~ 1
        private int _direction;

        #endregion
        /***********************************************************************
        *                           Look Up Tables
        ***********************************************************************/
        #region .
        private int _lookupCount;
        private int _lookupCenterIndex;

        private float[] _xPosTable; // 인덱스 위치에 따른 X 좌표 기록
        private Vector2[] _sizeTable; // 인덱스 위치에 따른 크기 기록

        private void GenerateLookUpTables()
        {
            _xPosTable = new float[_lookupCount];
            _sizeTable = new Vector2[_lookupCount];

            for (int i = 0; i < _lookupCount; i++)
            {
                _xPosTable[i] = GetXPosition(i);
                _sizeTable[i] = GetSize(i);
            }
        }

        /// <summary> lookup center index와의 인덱스 차이 계산 </summary>
        private int GetIndexDiffFromCenter(int index)
        {
            return Mathf.Abs(index - _lookupCenterIndex);
        }

        /// <summary> 가로세로 크기 구하기 </summary>
        private Vector2 GetSize(int lookupIndex)
        {
            if (lookupIndex == _lookupCenterIndex)
                return centerUISize;

            float absGap = GetIndexDiffFromCenter(lookupIndex);

            return Vector2.Lerp(centerUISize, edgeUISize, absGap / _lookupCenterIndex);
        }

        /// <summary> X좌표 구하기 </summary>
        private float GetXPosition(int lookupIndex)
        {
            // 중앙 위치
            if (lookupIndex == _lookupCenterIndex)
                return 0f;

            float absGap = GetIndexDiffFromCenter(lookupIndex);

            // // 1. 빈공간 합
            float pos = absGap * spaceWidth;

            // 2. 너비 합
            for (int i = 0; i <= absGap; i++)
            {
                float w = Vector2.Lerp(centerUISize, edgeUISize, i / (float)_lookupCenterIndex).x;

                if (0 < i && i < absGap)
                    pos += w;
                else
                    pos += w * 0.5f;
            }

            return (lookupIndex < _lookupCenterIndex) ? -pos : pos;
        }

        #endregion
        /***********************************************************************
        *                           Unity Events
        ***********************************************************************/
        #region .
        private void OnEnable()
        {
            _targetList.Clear();
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = transform.GetChild(i);
                GameObject go = child.gameObject;

                if (go.activeSelf == false) continue;
                if (go.hideFlags == HideFlags.HideInHierarchy) continue;

                _targetList.Add(child.GetComponent<RectTransform>());
            }

            _targetCount = _targetList.Count;
            _lookupCount = _targetCount + 2;
            _lookupCenterIndex = _lookupCount / 2;

            GenerateLookUpTables();
            InitRectTransforms();
            
            if (useImposter) GenerateImposters();
        }

        private void Update()
        {
            if (!_isTransiting)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    _direction = LEFT;
                    Local_BeginTransition();
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    _direction = RIGHT;
                    Local_BeginTransition();
                }
            }
            else
            {
                _progress += Time.deltaTime / transitionTime;
                if (_progress > 1f) _progress = 1f;

                OnTransition();

                if (_progress == 1f)
                {
                    _isTransiting = false;
                    _currentIndex = (_currentIndex - _direction) % _targetCount;
                    if (_currentIndex < 0) _currentIndex += _targetCount;
                    OnTransitionEnd();
                }
            }

            void Local_BeginTransition()
            {
                _isTransiting = true;
                _progress = 0;
                OnTransitionBegin();
            }
        }

        private void OnTransitionBegin()
        {
            if (useImposter) InitImposter();
        }

        private void OnTransition()
        {
            MoveAll();
            if (useImposter) MoveImposter();
        }

        private void OnTransitionEnd()
        {
            for (int i = 0; i < _targetCount; i++)
            {
                int li = GetLookupIndex(i);

                // 양측의 이미지를 알맞은 위치로 이동
                if (li == 1 || li == _targetCount)
                {
                    _targetList[i].SetSizeAndXPosition(_sizeTable[li], _xPosTable[li]);
                }
            }

            // 임포스터 비활성화
            if (useImposter)
                _currentImposter.gameObject.SetActive(false);
        }
        #endregion
        /***********************************************************************
        *                           Getter Methods
        ***********************************************************************/
        #region .
        /// <summary> index를 lookup index에 매핑하기 </summary>
        private int GetLookupIndex(int index)
        {
            index = index - _currentIndex + _lookupCenterIndex;

            if (index > _targetCount)
                index -= _targetCount;

            else if (index <= 0)
                index += _targetCount;

            return index;
        }

        /// <summary> lookup index를 index에 매핑하기 </summary>
        private int GetIndexFromLookupIndex(int lookupIndex)
        {
            int index = lookupIndex - _lookupCenterIndex + _currentIndex;

            if (index < 0)
                index += _targetCount;

            return index % _targetCount;
        }

        /// <summary> 가장 좌측 이미지의 실제 인덱스 </summary>
        private int GetLeftImageIndex() => GetIndexFromLookupIndex(0);
        /// <summary> 가장 우측 이미지의 실제 인덱스 </summary>
        private int GetRightImageIndex() => GetIndexFromLookupIndex(_lookupCount - 1);

        #endregion
        /***********************************************************************
        *                           Rect Transform Methods
        ***********************************************************************/
        #region .
        /// <summary> Rect Transform X Pos, Size 초기 설정 </summary>
        private void InitRectTransforms()
        {
            for (int i = 0; i < _targetCount; i++)
            {
                int li = GetLookupIndex(i);
                float xPos = _xPosTable[li];
                Vector2 size = _sizeTable[li];

                _targetList[i].SetSizeAndXPosition(size, xPos);
            }
        }

        /// <summary> 모든 이미지의 Rect Transform 이동시키기 </summary>
        private void MoveAll()
        {
            for (int i = 0; i < _targetCount; i++)
            {
                int li = GetLookupIndex(i);

                float curXPos = _xPosTable[li];
                float nextXPos = _xPosTable[li + _direction];
                float xPos = Mathf.Lerp(curXPos, nextXPos, _progress);

                Vector2 curSize = _sizeTable[li];
                Vector2 nextSize = _sizeTable[li + _direction];
                Vector2 size = Vector2.Lerp(curSize, nextSize, _progress);
                _targetList[i].SetSizeAndXPosition(size, xPos);
            }
        }
        #endregion
        /***********************************************************************
        *                           Imposter Methods
        ***********************************************************************/
        #region .
        private void GenerateImposters()
        {
            _imposterList.Clear();

            for (int i = 0; i < _targetCount; i++)
            {
                GameObject go = Instantiate(_targetList[i].gameObject);
                go.transform.SetParent(transform);
                go.hideFlags = HideFlags.HideInHierarchy;

                RectTransform rt = go.GetComponent<RectTransform>();
                _imposterList.Add(rt);
                go.SetActive(false);
            }
        }

        private void InitImposter()
        {
            int index, lookupIndex;
            float xPos;
            Vector2 size;

            switch (_direction)
            {
                default:
                case LEFT:
                    index = GetLeftImageIndex();
                    lookupIndex = 0;
                    break;

                case RIGHT:
                    index = GetRightImageIndex();
                    lookupIndex = _lookupCount - 1;
                    break;
            }

            xPos = _xPosTable[lookupIndex];
            size = _sizeTable[lookupIndex];

            _currentImposter = _imposterList[index];
            _currentImposter.SetSizeAndXPosition(size, xPos);
            _currentImposter.gameObject.SetActive(true);
        }

        private void MoveImposter()
        {
            int lookupIndex, lookupIndexNext;

            switch (_direction)
            {
                default:
                case LEFT:
                    lookupIndex = 0;
                    lookupIndexNext = 1;
                    break;

                case RIGHT:
                    lookupIndex = _lookupCount - 1;
                    lookupIndexNext = _lookupCount - 2;
                    break;
            }

            float curXPos = _xPosTable[lookupIndex];
            float nextXPos = _xPosTable[lookupIndexNext];
            float xPos = Mathf.Lerp(curXPos, nextXPos, _progress);

            Vector2 curSize = _sizeTable[lookupIndex];
            Vector2 nextSize = _sizeTable[lookupIndexNext];
            Vector2 size = Vector2.Lerp(curSize, nextSize, _progress);

            _currentImposter.SetSizeAndXPosition(size, xPos);
        }
        #endregion
        /***********************************************************************
        *                           Public Methods
        ***********************************************************************/
        #region .
        /// <summary> 현재 선택된 UI의 인덱스 참조 </summary>
        public int GetCurrentIndex() => _currentIndex;
        #endregion
    }

    /***********************************************************************
    *                               Extension Helpers
    ***********************************************************************/
    #region .
    static class RectTransformExtensionHelper
    {
        public static void SetSizeAndXPosition(this RectTransform @this, in Vector2 size, float xPos)
        {
            @this.sizeDelta = size;
            @this.anchoredPosition = new Vector2(xPos, 0f);
        }
    }

    #endregion
}