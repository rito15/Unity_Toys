using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rito.MeshGenerator
{
    // 정다각형 메시 생성
    public sealed class RegularPolygonMeshGenerator : MeshGeneratorBase
    {
        public float _radius = 2f;  // 중심에서 꼭짓점까지 거리
        public int _polygonVertex = 3;     // 꼭짓점 개수

        // Vertex, Triangle 계산
        protected override void CalculateMesh(out Vector3[] verts, out int[] tris)
        {
            Vector3 centerPoint = Vector3.zero; //transform.position;

            int vertsCount = _polygonVertex + 1;
            int trisCount = _polygonVertex * 3;

            verts = new Vector3[vertsCount];
            tris = new int[trisCount];

            Vector3 direction = Vector3.forward;
            Vector3 vertPoint = centerPoint + direction * _radius; // 다각형 위의 버텍스

            // 1. 버텍스 초기화
            verts[0] = centerPoint;
            for (int i = 1; i < vertsCount; i++)
            {
                verts[i] = vertPoint;

                // 회전하여 다음 버텍스 지점 찾기
                direction = Quaternion.Euler(0f, 360f / _polygonVertex, 0f) * direction;
                vertPoint = centerPoint + direction * _radius;
            }

            // 2. 트리스 초기화
            for (int i = 0; i < _polygonVertex; i++)
            {
                tris[i * 3] = 0;
                tris[i * 3 + 1] = i + 1;
                tris[i * 3 + 2] = i + 2;
            }

            // 트리스 마지막 버텍스 오버플로 해결
            tris[_polygonVertex * 3 - 1] = 1;
        }
    }
}