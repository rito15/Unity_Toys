using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    #region Fields

    public float _viewRadius = 5f;
    [Range(1f, 361f)]
    public float _viewAngle = 360f;
    public float _meshResolution = 1f;    // 뷰캐스트 메시 해상도 -> 성능에 영향
    public int _edgeResolveIteration = 4; // 꼭지점 탐색 정확도 -> 성능에 영향
    public float _edgeDstThreshold = 5f;

    public float _targetFindDelay = 0.2f; // 타겟 탐색 주기

    /// <summary> 레이어마스크 지정 </summary>
    [SerializeField]
    private Mask_ _mask = new Mask_();

    public MeshFilter _viewMeshFiter;
    private Mesh _viewMesh;

    public float _maskCutawayDistance = 0.1f; // 스텐실 마스크로 잘라냈을 때 장애물 가장자리가 살짝 보이도록 할 경계 두께

    [HideInInspector]
    public List<Transform> _visibleTargets = new List<Transform>();

    #endregion // ==========================================================

    #region Unity Callbacks

    private void Start()
    {
        _viewMesh = new Mesh();
        _viewMesh.name = "FOV Mesh";
        _viewMeshFiter.mesh = _viewMesh;

        StartCoroutine(FindTargetsWithDealyRoutine(_targetFindDelay));
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    #endregion // ==========================================================

    #region Public Methods

    /*

    원래는

         90
    180 원점 0
        270
    
    에서 Vector2(Cos(angle), Sin(angle))으로 Normalized Direction Vector를 얻어내지만,

         0
    270 원점 90
        180

    을 구해야 하므로

    90도를 더해주고
    -angle을 해주면

    angle -> (90 - angle)

    따라서

    Cos(90 - angle) == Sin(angle)
    Sin(90 - angle) == Cos(angle)

    */

    public Vector3 DirectionFromAngle(float angleInDegrees, bool isAngleGlobal)
    {
        if (!isAngleGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(
            Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),
            0f,
            Mathf.Cos(angleInDegrees * Mathf.Deg2Rad)
        );
    }

    #endregion // ==========================================================

    #region Private Methods

    // OverlapSphere, Raycast 이용
    // 시야 내에 들어온 적들 탐색
    private void FindVisibleTargets()
    {
        _visibleTargets.Clear();

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, _viewRadius, _mask.Target);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            // 시야 각도 제한
            if (Vector3.Angle(transform.forward, dirToTarget) < _viewAngle * 0.5f)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                // 플레이어 - [거리] -> 타겟
                // 장애물 레이어로 레이캐스트 해서 장애물이 존재하지 않는다면
                // VisibleTarget 목록에 추가
                if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, _mask.Obstacle))
                {
                    _visibleTargets.Add(target);
                }
            }
        }
    }

    // 레이캐스트를 통해 정점을 만들고, 각각 연결하여 메시 생성
    private void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(_viewAngle * _meshResolution);
        float stepAngleSize = _viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i < stepCount; i++)
        {
            // 매 시행의 각도(플레이어의 Y회전각을 중심으로 viewAngle 사이에서 stepAngleSize만큼 증가) 계산
            float angle = transform.eulerAngles.y - _viewAngle * 0.5f + stepAngleSize * i;

            //Debug.DrawLine(transform.position, transform.position + DirectionFromAngle(angle, true) * _viewRadius, Color.red);

            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > _edgeDstThreshold;
                if (oldViewCast.hit != newViewCast.hit ||
                    oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded)
                {
                    // 각각의 뷰캐스트 사이에서 장애물의 꼭지점을 찾아 해당 정점을 viewPoints에 추가 
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);

                    if (edge.pointA != Vector3.zero)
                        viewPoints.Add(edge.pointA);

                    if (edge.pointB != Vector3.zero)
                        viewPoints.Add(edge.pointB);
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast; // 이번 시행의 뷰캐스트를 Old에 저장
        }

        // 정점을 모아 삼각형을 만들기
        // X : 공통 정점, A ~ : 나머지 2개 정점
        // XAB, XBC, XCD, XDE, ... 꼴로 정점을 모아 삼각형을 형성하므로
        // 생성되는 삼각형 개수 = (정점 개수 - 2)
        int vertexCount = viewPoints.Count + 1; // 정점 개수 : 뷰캐스트 종착점 개수 + 캐스팅 원점
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3]; // 삼각형 개수 * 3 => 모든 개별 삼각형들의 정점 개수(중첩)
                                                          // => 배열 내에 각각의 정점 인덱스를 저장

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]) + Vector3.forward * _maskCutawayDistance;

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        // 메시 그려주기
        _viewMesh.Clear();
        _viewMesh.vertices = vertices;
        _viewMesh.triangles = triangles;
        _viewMesh.RecalculateNormals();
    }

    // 장애물에 레이캐스트하여 메시의 정점이 될 위치 찾기
    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirectionFromAngle(globalAngle, true);

        if (Physics.Raycast(transform.position, dir, out var hit, _viewRadius, _mask.Obstacle))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * _viewRadius, _viewRadius, globalAngle);
        }
    }

    // 뷰캐스트 사이에서 장애물의 꼭지점 찾아내기
    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < _edgeResolveIteration; i++)
        {
            float angle = (minAngle + maxAngle) * 0.5f;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.distance - maxViewCast.distance) > _edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    #endregion // ==========================================================

    #region Coroutines

    IEnumerator FindTargetsWithDealyRoutine(float delay)
    {
        var wfs = new WaitForSeconds(delay);

        while (true)
        {
            yield return wfs;
            FindVisibleTargets();
        }
    }

    #endregion // ==========================================================

    #region Classes, Structs

    [System.Serializable]
    public class Mask_
    {
        public LayerMask Obstacle => _obstacle;
        public LayerMask Target => _target;

        [SerializeField]
        private LayerMask _obstacle;
        [SerializeField]
        private LayerMask _target;
    }

    /// <summary> 특정 방향으로 레이캐스트하여, 정보 얻어오기
    /// <para/> - 장애물에 레이가 닿았는지 여부
    /// <para/> - 레이 도착 지점
    /// <para/> - 레이의 거리
    /// <para/> - 회전 시작 지점으로부터의 각도
    /// </summary>
    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool hit, Vector3 point, float distance, float angle)
        {
            this.hit = hit;
            this.point = point;
            this.distance = distance;
            this.angle = angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 pointA, Vector3 pointB)
        {
            this.pointA = pointA;
            this.pointB = pointB;
        }
    }

    #endregion // ==========================================================
}
