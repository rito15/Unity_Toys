using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 날짜 : 2021-08-10 PM 8:47:57
// 작성자 : Rito

namespace Rito
{
    /// <summary> 
    /// 렌더 텍스쳐를 이용해 땅에 눈 쌓기
    /// </summary>
    public class GroundSnowPainter : MonoBehaviour
    {
        [SerializeField]
        private Material targetMaterial; // 렌더 텍스쳐를 메인 텍스쳐로 적용할 대상 마테리얼

        [SerializeField, Range(0.01f, 1f)]
        private float brushSize = 0.1f;

        [SerializeField, Range(0.01f, 1f)]
        private float pileBrushIntensity = 0.1f;

        [SerializeField, Range(0.01f, 1f)]
        private float eraserBrushIntensity = 0.5f;

        [SerializeField] // 인스펙터 확인용
        private RenderTexture snowRenderTexture; // 브러시로 그려질 대상 렌더 텍스쳐

        private Texture2D whiteBrushTexture; // Painter
        private Texture2D blackBrushTexture; // Eraser

        private const int Resolution = 1024;

        private void Awake()
        {
            snowRenderTexture = new RenderTexture(Resolution, Resolution, 0);
            snowRenderTexture.filterMode = FilterMode.Point;
            snowRenderTexture.Create();

            targetMaterial.mainTexture = snowRenderTexture;

            whiteBrushTexture = CreateBrushTexture(Color.white, pileBrushIntensity);
            blackBrushTexture = CreateBrushTexture(Color.black, eraserBrushIntensity);
        }

        private void OnApplicationQuit()
        {
            if(snowRenderTexture) Destroy(snowRenderTexture);
            if(whiteBrushTexture) Destroy(whiteBrushTexture);
            if(blackBrushTexture) Destroy(blackBrushTexture);
        }

        private Texture2D CreateBrushTexture(Color color, float intensity)
        {
            int res = Resolution / 2;
            float hRes = res * 0.5f;
            float sqrSize = hRes * hRes;

            Texture2D texture = new Texture2D(res, res);
            texture.filterMode = FilterMode.Bilinear;

            for (int y = 0; y < res; y++)
            {
                for (int x = 0; x < res; x++)
                {
                    // Sqaure Length From Center
                    float sqrLen = (hRes - x) * (hRes - x) + (hRes - y) * (hRes - y);
                    float alpha = Mathf.Max(sqrSize - sqrLen, 0f) / sqrSize;

                    // Soft
                    alpha = Mathf.Pow(alpha, 2f);

                    color.a = alpha * intensity;
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            return texture;
        }

        /// <summary> 렌더 텍스쳐에 브러시 텍스쳐로 그리기 </summary>
        private void PaintBrush(Texture2D brush, Vector2 uv, float size)
        {
            RenderTexture.active = snowRenderTexture;         // 페인팅을 위해 활성 렌더 텍스쳐 임시 할당
            GL.PushMatrix();                                  // 매트릭스 백업
            GL.LoadPixelMatrix(0, Resolution, Resolution, 0); // 알맞은 크기로 픽셀 매트릭스 설정

            float brushPixelSize = brushSize * Resolution * size;
            uv.x *= Resolution;
            uv.y *= Resolution;

            // 렌더 텍스쳐에 브러시 텍스쳐를 이용해 그리기
            Graphics.DrawTexture(
                new Rect(
                    uv.x - brushPixelSize * 0.5f,
                    (snowRenderTexture.height - uv.y) - brushPixelSize * 0.5f,
                    brushPixelSize,
                    brushPixelSize
                ),
                brush
            );

            GL.PopMatrix();              // 매트릭스 복구
            RenderTexture.active = null; // 활성 렌더 텍스쳐 해제
        }

        /// <summary> 눈 쌓기 </summary>
        public void PileSnow(Vector3 contactPoint)
        {
            float snowSize = UnityEngine.Random.Range(0.5f, 2.0f);
            Paint(contactPoint, snowSize, true);
        }

        /// <summary> 눈 지우기 </summary>
        public void ClearSnow(Vector3 contactPoint, float size)
        {
            Paint(contactPoint, size, false);
        }

        /// <summary> 눈 쌓기 or 지우기 </summary>
        private void Paint(in Vector3 contactPoint, float size = 1f, bool pileOrClear = true)
        {
            // 눈이 부딪힌 3D 좌표로부터 2D UV 좌표 계산
            // Plane은 scale 1당 좌표 10이므로 10으로 나누기
            Vector3 normalizedVec3 = (contactPoint - transform.position) / 10f;
            normalizedVec3.x /= transform.lossyScale.x;
            normalizedVec3.z /= transform.lossyScale.z;

            Vector2 uv = new Vector2(normalizedVec3.x + 0.5f, normalizedVec3.z + 0.5f);

            // UV 범위 바깥이면 배제
            if (uv.x < 0f || uv.y < 0f || uv.x > 1f || uv.y > 1f)
                return;

            uv = Vector2.one - uv; // 좌표 반전

            // 1. 쌓기
            if (pileOrClear)
            {
                PaintBrush(whiteBrushTexture, uv, size);
            }
            // 2. 지우기
            else
            {
                PaintBrush(blackBrushTexture, uv, size);
            }
        }
    }
}