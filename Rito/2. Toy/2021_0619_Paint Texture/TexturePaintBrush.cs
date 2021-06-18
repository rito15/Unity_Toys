// https://www.patreon.com/posts/rendertexture-15961186
// https://pastebin.com/rMx1PVXi

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-06-18 AM 2:30:31
// 작성자 : Rito

namespace Rito.TexturePainter
{
    /// <summary> 마우스 드래그로 텍스쳐에 그림 그리기 </summary>
    [DisallowMultipleComponent]
    public class TexturePaintBrush : MonoBehaviour
    {
        /***********************************************************************
        *                               Public Fields
        ***********************************************************************/
        #region .

        [Range(0.01f, 1f)] public float brushSize = 0.1f;
        public Texture2D brushTexture;
        public Color brushColor = Color.white;

        #endregion
        /***********************************************************************
        *                               Private Fields
        ***********************************************************************/
        #region .

        private TexturePaintTarget paintTarget;
        private Collider prevCollider;

        private Texture2D CopiedBrushTexture; // 실시간으로 색상 칠하는데 사용되는 브러시 텍스쳐 카피본
        private Vector2 sameUvPoint; // 직전 프레임에 마우스가 위치한 대상 UV 지점 (동일 위치에 중첩해서 그리는 현상 방지)

        #endregion

        /***********************************************************************
        *                               Unity Events
        ***********************************************************************/
        #region .
        private void Awake()
        {
            // 등록한 브러시 텍스쳐가 없을 경우, 원 모양의 텍스쳐 생성
            if (brushTexture == null)
            {
                CreateDefaultBrushTexture();
            }

            CopyBrushTexture();
        }

        private void Update()
        {
            UpdateBrushColorOnEditor();

            if (Input.GetMouseButton(0) == false) return;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit)) // delete previous and uncomment for mouse painting
            {
                Collider currentCollider = hit.collider;
                if (currentCollider != null)
                {
                    // 대상 참조 갱신
                    if (prevCollider == null || prevCollider != currentCollider)
                    {
                        prevCollider = currentCollider;
                        currentCollider.TryGetComponent(out paintTarget);
                    }

                    // 동일한 지점에는 중첩하여 다시 그리지 않음
                    if (sameUvPoint != hit.lightmapCoord)
                    {
                        sameUvPoint = hit.lightmapCoord;
                        Vector2 pixelUV = hit.lightmapCoord;
                        pixelUV.x *= paintTarget.resolution;
                        pixelUV.y *= paintTarget.resolution;
                        paintTarget.DrawTexture(pixelUV.x, pixelUV.y, brushSize, CopiedBrushTexture);
                    }
                }
            }
        }
        #endregion
        /***********************************************************************
        *                               Public Methods
        ***********************************************************************/
        #region .
        /// <summary> 브러시 색상 변경 </summary>
        public void SetBrushColor(in Color color)
        {
            brushColor = color;
            CopyBrushTexture();
        }

        #endregion
        /***********************************************************************
        *                               Private Methods
        ***********************************************************************/
        #region .

        /// <summary> 기본 형태(원)의 브러시 텍스쳐 생성 </summary>
        private void CreateDefaultBrushTexture()
        {
            int res = 512;
            float hRes = res * 0.5f;
            float sqrSize = hRes * hRes;

            brushTexture = new Texture2D(res, res);
            brushTexture.filterMode = FilterMode.Point;
            brushTexture.alphaIsTransparency = true;

            for (int y = 0; y < res; y++)
            {
                for (int x = 0; x < res; x++)
                {
                    // Sqaure Length From Center
                    float sqrLen = (hRes - x) * (hRes - x) + (hRes - y) * (hRes - y);
                    float alpha = Mathf.Max(sqrSize - sqrLen, 0f) / sqrSize;

                    //brushTexture.SetPixel(x, y, (sqrLen < sqrSize ? brushColor : Color.clear));
                    brushTexture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            brushTexture.Apply();
        }

        /// <summary> 원본 브러시 텍스쳐 -> 실제 브러시 텍스쳐(색상 적용) 복제 </summary>
        private void CopyBrushTexture()
        {
            if (brushTexture == null) return;

            // 기존의 카피 텍스쳐는 메모리 해제
            DestroyImmediate(CopiedBrushTexture);

            // 새롭게 할당
            {
                CopiedBrushTexture = new Texture2D(brushTexture.width, brushTexture.height);
                CopiedBrushTexture.filterMode = FilterMode.Point;
                CopiedBrushTexture.alphaIsTransparency = true;
            }

            int height = brushTexture.height;
            int width = brushTexture.width;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color c = brushColor;
                    c.a *= brushTexture.GetPixel(x, y).a;

                    CopiedBrushTexture.SetPixel(x, y, c);
                }
            }

            CopiedBrushTexture.Apply();

            Debug.Log("Copy Brush Texture");
        }

        #endregion
        /***********************************************************************
        *                               Editor Only
        ***********************************************************************/
        #region .
#if UNITY_EDITOR
        // 색상 변경 감지하여 브러시 텍스쳐 다시 복제
        private Color prevBrushColor;
        private float brushTextureUpdateCounter = 0f;
        private const float BrushTextureUpdateCounterInitValue = 0.7f;
        private void OnValidate()
        {
            if (Application.isPlaying && prevBrushColor != brushColor)
            {
                brushTextureUpdateCounter = BrushTextureUpdateCounterInitValue;
                prevBrushColor = brushColor;
            }
        }
#endif
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void UpdateBrushColorOnEditor()
        {
            if (brushTextureUpdateCounter > 0f && 
                brushTextureUpdateCounter <= BrushTextureUpdateCounterInitValue)
            {
                brushTextureUpdateCounter -= Time.deltaTime;
            }

            if(brushTextureUpdateCounter < 0f)
            {
                CopyBrushTexture();
                brushTextureUpdateCounter = 9999f;
            }
        }
        #endregion

    }
}