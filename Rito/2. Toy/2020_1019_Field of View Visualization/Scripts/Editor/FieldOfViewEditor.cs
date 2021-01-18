using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        Transform transform = fov.transform;
        Vector3 playerPos = transform.position;

        // 시야 반경 원 그리기
        Handles.color = Color.white;
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, fov._viewRadius);

        Vector3 viewAngleEdgeA = fov.DirectionFromAngle(-fov._viewAngle * 0.5f, false);
        Vector3 viewAngleEdgeB = fov.DirectionFromAngle( fov._viewAngle * 0.5f, false);

        // 시야 각도 직선 2개 그리기
        Handles.color = Color.yellow;
        Handles.DrawLine(playerPos, playerPos + viewAngleEdgeA * fov._viewRadius);
        Handles.DrawLine(playerPos, playerPos + viewAngleEdgeB * fov._viewRadius);

        // 현재 보이는 적으로부터 플레이어에게 레이 그리기
        Handles.color = Color.red;
        fov._visibleTargets.ForEach(target =>
        {
            Handles.DrawLine(target.position, playerPos);
        });
    }
}
