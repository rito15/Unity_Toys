using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 날짜 : 2021-06-19 AM 3:00:38
// 작성자 : Rito

namespace Rito.TexturePainter
{
    /*
     * [NOTE]
     * 
     * - 반드시 Rito/PaintTexture 마테리얼 사용
     * - 마테리얼의 Enable GPU Instancing 체크
     * 
     */

    /// <summary> 
    /// 그림 그려질 대상
    /// </summary>
    [DisallowMultipleComponent]
    public class TexturePaintTarget : MonoBehaviour
    {
        /***********************************************************************
        *                               Static Fields
        ***********************************************************************/
        #region .
        private static Texture2D ClearTex
        {
            get
            {
                if (_clearTex == null)
                {
                    _clearTex = new Texture2D(1, 1);
                    _clearTex.SetPixel(0, 0, Color.clear);
                    _clearTex.Apply();
                }
                return _clearTex;
            }
        }
        private MaterialPropertyBlock TextureBlock
        {
            get
            {
                if (_textureBlock == null)
                {
                    _textureBlock = new MaterialPropertyBlock();
                }
                return _textureBlock;
            }
        }

        private static Texture2D _clearTex;
        private MaterialPropertyBlock _textureBlock;

        private static readonly string PaintTexPropertyName = "_PaintTex";

        #endregion
        /***********************************************************************
        *                               Private Fields
        ***********************************************************************/
        #region .
        private MeshRenderer _mr;

        #endregion
        /***********************************************************************
        *                               Public Fields
        ***********************************************************************/
        #region .
        public int resolution = 512;
        public RenderTexture renderTexture = null;

        #endregion
        /***********************************************************************
        *                               Unity Magics
        ***********************************************************************/
        #region .
        private void Awake()
        {
            Init();
            InitRenderTexture();
        }

        #endregion
        /***********************************************************************
        *                               Private Methods
        ***********************************************************************/
        #region .

        private void Init()
        {
            TryGetComponent(out _mr);
        }

        /// <summary> 렌더 텍스쳐 초기화 </summary>
        private void InitRenderTexture()
        {
            renderTexture = new RenderTexture(resolution, resolution, 32);
            Graphics.Blit(ClearTex, renderTexture);

            // 마테리얼 프로퍼티 블록 이용하여 배칭 유지하고
            // 마테리얼의 프로퍼티에 렌더 텍스쳐 넣어주기
            TextureBlock.SetTexture(PaintTexPropertyName, renderTexture);
            _mr.SetPropertyBlock(TextureBlock);
        }

        #endregion
        /***********************************************************************
        *                               Public Methods
        ***********************************************************************/
        #region .
        /// <summary> 렌더 텍스쳐에 브러시 텍스쳐로 그리기 </summary>
        public void DrawTexture(float posX, float posY, float brushSize, Texture2D brushTexture)
        {
            RenderTexture.active = renderTexture; // 페인팅을 위해 활성 렌더 텍스쳐 임시 할당
            GL.PushMatrix();                      // 매트릭스 저장
            GL.LoadPixelMatrix(0, resolution, resolution, 0); // 알맞은 크기로 픽셀 매트릭스 설정

            float brushPixelSize = brushSize * resolution;

            // 렌더 텍스쳐에 브러시 텍스쳐를 이용해 그리기
            Graphics.DrawTexture(
                new Rect(
                    posX - brushPixelSize * 0.5f,
                    (renderTexture.height - posY) - brushPixelSize * 0.5f,
                    brushPixelSize,
                    brushPixelSize
                ),
                brushTexture
            );

            GL.PopMatrix();              // 매트릭스 복구
            RenderTexture.active = null; // 활성 렌더 텍스쳐 해제
        }

        #endregion
    }
}