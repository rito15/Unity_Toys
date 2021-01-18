using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2021-01-09 21:45

namespace Rito.MeshGenerator
{
    public class PerlinNoiseMeshGenerator : MeshGeneratorBase
    {
        public Vector2Int _resolution = new Vector2Int(24, 24);
        public Vector2 _width = new Vector2(10f, 10f);
        public float _minHeight = 0f;
        public float _maxHeight = 1f;
        public float _noiseDensity = 1f;

        public bool _randomize = false;
        public bool _addRandomSmallNoises = false;
        public float _smallNoiseRange = 0.1f;

        protected override void CalculateMesh(out Vector3[] verts, out int[] tris)
        {
            Vector3 widthV3 = new Vector3(_width.x, 0f, _width.y); // width를 3D로 변환
            Vector3 startPoint = -widthV3 * 0.5f;                  // 첫 버텍스의 위치
            Vector2 gridUnit = _width / _resolution;               // 그리드 하나의 너비

            Vector2Int vCount = new Vector2Int(_resolution.x + 1, _resolution.y + 1); // 각각 가로, 세로 버텍스 개수
            int vertsCount = vCount.x * vCount.y;
            int trisCount = _resolution.x * _resolution.y * 6;

            verts = new Vector3[vertsCount];
            tris = new int[trisCount];

            Vector2 randomOffset = Vector2.zero;
            if (_randomize)
            {
                randomOffset = new Vector2(
                    randomOffset.x + Random.Range(0.001f, 10.001f),
                    randomOffset.y + Random.Range(11.001f, 31.001f)
                );
            }

            // 1. 버텍스 초기화
            for (int j = 0; j < vCount.y; j++)
            {
                for (int i = 0; i < vCount.x; i++)
                {
                    int index = i + j * vCount.x;
                    verts[index] = startPoint
                        + new Vector3(
                            gridUnit.x * i,
                            GetPerlinNoiseHeight(i, j, randomOffset),
                            gridUnit.y * j
                        );
                }
            }

            // 2. 트리스 초기화
            int tIndex = 0;
            for (int j = 0; j < vCount.y - 1; j++)
            {
                for (int i = 0; i < vCount.x - 1; i++)
                {
                    int vIndex = i + j * vCount.x;

                    tris[tIndex + 0] = vIndex;
                    tris[tIndex + 1] = vIndex + vCount.x;
                    tris[tIndex + 2] = vIndex + 1;

                    tris[tIndex + 3] = vIndex + vCount.x;
                    tris[tIndex + 4] = vIndex + vCount.x + 1;
                    tris[tIndex + 5] = vIndex + 1;

                    tIndex += 6;
                }
            }

            float GetPerlinNoiseHeight(int i, int j, Vector2 offset)
            {
                float a = i * 10f / _resolution.x;
                float b = j * 10f / _resolution.y;

                if (_randomize)
                {
                    a += offset.x;
                    b += offset.y;
                }

                a *= _noiseDensity;
                b *= _noiseDensity;

                float noiseHeight =
                    Mathf.PerlinNoise(a, b)
                    * (_maxHeight - _minHeight);

                if (_addRandomSmallNoises)
                {
                    noiseHeight += Random.Range(-_smallNoiseRange, _smallNoiseRange);
                }

                float noise = _minHeight + noiseHeight;

                //Debug.Log($"{a:F3}, {b:F3}");

                return noise;
            }
        }
    }
}