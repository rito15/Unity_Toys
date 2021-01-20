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

        public int _randomSeed = 0;
        public int _randomSmallSeed = 0;

        public bool _addRandomSmallNoises = false;
        public float _smallNoiseRange = 0.1f;

        protected override void CalculateMesh(out Vector3[] verts, out int[] tris)
        {
            Vector3 widthV3 = new Vector3(_width.x, 0f, _width.y); // width�� 3D�� ��ȯ
            Vector3 startPoint = -widthV3 * 0.5f;                  // ù ���ؽ��� ��ġ
            Vector2 gridUnit = _width / _resolution;               // �׸��� �ϳ��� �ʺ�

            Vector2Int vCount = new Vector2Int(_resolution.x + 1, _resolution.y + 1); // ���� ����, ���� ���ؽ� ����
            int vertsCount = vCount.x * vCount.y;
            int trisCount = _resolution.x * _resolution.y * 6;

            verts = new Vector3[vertsCount];
            tris = new int[trisCount];

            // Small Random
            float[] randomSmallNoiseHeights = new float[vertsCount];
            if (_addRandomSmallNoises)
            {
                Random.InitState(_randomSmallSeed);
                for (int i = 0; i < randomSmallNoiseHeights.Length; i++)
                {
                    randomSmallNoiseHeights[i] = Random.Range(-_smallNoiseRange, _smallNoiseRange);
                }
            }

            // Ground Seed
            Random.InitState(_randomSeed);
            Vector2 randomOffset = new Vector2(
                    Random.Range(0.001f, 10.001f),
                    Random.Range(11.001f, 31.001f)
                );

            // 1. ���ؽ� �ʱ�ȭ
            for (int j = 0; j < vCount.y; j++)
            {
                for (int i = 0; i < vCount.x; i++)
                {
                    int index = i + j * vCount.x;
                    verts[index] = startPoint
                        + new Vector3(
                            gridUnit.x * i,
                            GetPerlinNoiseHeight(i, j, randomOffset)
                            + (_addRandomSmallNoises ? randomSmallNoiseHeights[index] : 0f)
                            ,
                            gridUnit.y * j
                        );
                }
            }

            // 2. Ʈ���� �ʱ�ȭ
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

            float GetPerlinNoiseHeight(int i, int j, Vector2 ranOffset)
            {
                float a = i * 10f / _resolution.x;
                float b = j * 10f / _resolution.y;

                a += ranOffset.x;
                b += ranOffset.y;

                a *= _noiseDensity;
                b *= _noiseDensity;

                float noiseHeight =
                    Mathf.PerlinNoise(a, b)
                    * (_maxHeight - _minHeight);

                float noise = _minHeight + noiseHeight;

                //Debug.Log($"{a:F3}, {b:F3}");

                return noise;
            }
        }
    }
}