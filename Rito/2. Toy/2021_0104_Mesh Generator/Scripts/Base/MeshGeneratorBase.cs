using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rito.MeshGenerator
{
    // 설명 : 실시간으로 커스텀 메시 생성
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public abstract class MeshGeneratorBase : MonoBehaviour
    {
        public bool _showVertexGizmo = false;
        public bool _showEdgeGizmo = false;

        protected MeshFilter _meshFilter;
        protected Mesh _mesh;

        protected Vector3[] _verts;

        public virtual void GenerateMesh()
        {
            TryGetComponent(out _meshFilter);
            _mesh = new Mesh();
            _meshFilter.mesh = _mesh;

            CalculateMesh(out _verts, out var tris);

            _mesh.Clear();
            _mesh.vertices = _verts;
            _mesh.triangles = tris;
            _mesh.RecalculateNormals();
            //_mesh.RecalculateBounds();
        }

        protected virtual void Awake()
        {
            TryGetComponent(out _meshFilter);
            _mesh = _meshFilter.mesh;
            _verts = _mesh.vertices;
        }

        protected abstract void CalculateMesh(out Vector3[] verts, out int[] tris);

        private void OnDrawGizmosSelected()
        {
            if (_mesh == null) return;

            if (_showVertexGizmo)
            {
                Vector3 pos = transform.position;
                foreach (var vertex in _mesh.vertices)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawSphere(vertex + pos, 0.05f);
                }
            }

            if (_showEdgeGizmo)
            {
                Vector3 pos = transform.position;
                for (int i = 0; i < _mesh.triangles.Length; i += 3)
                {
                    int v0 = _mesh.triangles[i];
                    int v1 = _mesh.triangles[i + 1];
                    int v2 = _mesh.triangles[i + 2];

                    Gizmos.color = Color.black;
                    Gizmos.DrawLine(_mesh.vertices[v0] + pos, _mesh.vertices[v1] + pos);
                    Gizmos.DrawLine(_mesh.vertices[v1] + pos, _mesh.vertices[v2] + pos);
                    Gizmos.DrawLine(_mesh.vertices[v2] + pos, _mesh.vertices[v0] + pos);
                }
            }
        }
    }
}