using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rito.MeshGenerator
{
    // 정다각기둥 메시 생성
    public class PrismMeshGenerator : MeshGeneratorBase
    {
        public float _topRadius = 1f;
        public float _bottomRadius = 2f;
        public float _height = 3f;
        public int _polygonVertex = 3;     // 윗면, 아랫면 다각형의 꼭짓점 개수
        public PivotPoint _pivotPoint = PivotPoint.Bottom;

        public enum PivotPoint
        {
            Top, Center, Bottom
        }

        protected override void CalculateMesh(out Vector3[] verts, out int[] tris)
        {
            Vector3 botCenterPoint = Vector3.zero;
            Vector3 topCenterPoint = botCenterPoint + Vector3.up * _height;

            // 피벗 재조정
            float pivotHeight = 0f;
            switch (_pivotPoint)
            {
                case PivotPoint.Top: pivotHeight = _height; break;
                case PivotPoint.Center: pivotHeight = _height * 0.5f; break;
            }
            botCenterPoint -= Vector3.up * pivotHeight;
            topCenterPoint -= Vector3.up * pivotHeight;

            int pvCount = _polygonVertex;

            int vertsCount = (pvCount + 1) * 2;
            int trisCount = pvCount * 3 * 4; // 꼭짓점개수 * (윗면1 + 아랫면1 + 옆면2)

            verts = new Vector3[vertsCount];
            tris = new int[trisCount];

            Vector3 direction = Vector3.forward;
            Vector3 botVertPoint = botCenterPoint + direction * _bottomRadius; // 아랫면 꼭짓점
            Vector3 topVertPoint = topCenterPoint + direction * _topRadius; // 윗면 꼭짓점

            // 1. 버텍스 초기화
            verts[0] = botCenterPoint; // 첫 버텍스 : 아랫면 중심지점
            verts[vertsCount - 1] = topCenterPoint; // 마지막 버텍스 : 윗면 중심지점

            for (int i = 1; i <= pvCount; i++)
            {
                verts[i] = botVertPoint;                  // 아랫면
                verts[i + pvCount] = topVertPoint; // 윗면

                // 회전하여 다음 버텍스 지점 찾기
                direction = Quaternion.Euler(0f, 360f / pvCount, 0f) * direction;
                botVertPoint = botCenterPoint + direction * _bottomRadius;
                topVertPoint = topCenterPoint + direction * _topRadius;
            }

            // 2. 트리스 초기화
            // 2-1. 아랫면
            for (int i = 0; i < pvCount; i++)
            {
                tris[i * 3 + 1] = 0;
                tris[i * 3] = i + 1;
                tris[i * 3 + 2] = i + 2;
            }
            tris[pvCount * 3 - 1] = 1;

            // 2-2. 윗면
            for (int i = pvCount; i < pvCount * 2; i++)
            {
                tris[i * 3] = vertsCount - 1;
                tris[i * 3 + 1] = i + 1;
                tris[i * 3 + 2] = i + 2;
            }
            tris[pvCount * 6 - 1] = pvCount + 1;

            // 2-3. 옆면
            for (int i = 0; i < pvCount; i++)
            {
                // i : 1 ~ pvCount (아랫면 버텍스 인덱스)
                // j : i + pvCount (윗면 버텍스 인덱스)
                // a, b : 옆면 트리스 인인덱스

                int j = i + pvCount;
                int a = i + pvCount * 2;
                int b = i + pvCount * 3;

                tris[a * 3] = i + 1 + pvCount;
                tris[a * 3 + 1] = i + 1;
                tris[a * 3 + 2] = (i + 2 <= pvCount) ? i + 2 : (i + 2) % pvCount;

                tris[b * 3] = (i + 2 <= pvCount) ? i + 2 : (i + 2) % pvCount;
                tris[b * 3 + 1] = (j + 2 <= pvCount * 2) ? j + 2 : (j + 2) - pvCount;
                tris[b * 3 + 2] = j + 1;
            }
        }
    }
}