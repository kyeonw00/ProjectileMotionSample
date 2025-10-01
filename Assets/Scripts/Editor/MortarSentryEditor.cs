/*
 * -----------------------------------------------------------------------------
 * Portfolio Rewritten Code
 *
 * 이 코드는 Buff Studio Inc.에서 작성된 원본 코드를
 * 기반으로 개인 포트폴리오 용도로 재작성한 것입니다.
 * 
 * 본 코드에는 기밀 정보가 포함되어 있지 않으며, 원본 프로젝트와는 별개로 동작합니다.
 *
 * Copyright (c) 강병준(github: kyeonw00), 
 * Licensed for personal portfolio and demonstration purposes only.
 * -----------------------------------------------------------------------------
 */

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MortarSentry))]
public class MortarSentryEditor : Editor
{
    private MortarSentry m_MortarSentry;
    
    private readonly Vector3[] m_MinimumRangeTrajectoryPoints = new Vector3[16];
    private readonly Vector3[] m_MaximumRangeTrajectoryPoints = new Vector3[16];

    private void OnEnable()
    {
        m_MortarSentry = (MortarSentry)target;
        
        UpdateMinMaxRangeTrajectories();
    }

    private void OnValidate()
    {
        UpdateMinMaxRangeTrajectories();
    }

    private void OnSceneGUI()
    {
        var transform = m_MortarSentry.transform;

        // 최소 - 최대 공격 사거리 범위 비주얼라이징
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, transform.up, m_MortarSentry.AttackRange);
        Handles.DrawWireDisc(transform.position, transform.up, m_MortarSentry.MinAttackRange);
        
        // 최소 사거리 궤적 비주얼라이징
        DrawTrajectoryWithLabel(
            m_MinimumRangeTrajectoryPoints, "Min Range", Color.magenta, 2f);
        
        // 최대 사거리 궤적 비주얼라이징
        DrawTrajectoryWithLabel(
            m_MaximumRangeTrajectoryPoints, "Max Range", Color.cyan, 2f);
    }

    private void UpdateMinMaxRangeTrajectories()
    {
        var transform = m_MortarSentry.transform;
        var gravity = m_MortarSentry.ProjectileGravity;
        var timeOfFlight = m_MortarSentry.ProjectileTimeOfFlight;
        
        // 최소 거리 궤적 계산
        var minRangeDestination = transform.position + transform.forward * m_MortarSentry.MinAttackRange;
        PhysicsUtils.GetTrajectoryPoints(
            transform.position, minRangeDestination, gravity, timeOfFlight,
            m_MinimumRangeTrajectoryPoints);
        
        // 최대 거리 궤적 계산
        var maxRangeDestination = transform.position + transform.forward * m_MortarSentry.AttackRange;
        PhysicsUtils.GetTrajectoryPoints(
            transform.position, maxRangeDestination, gravity, timeOfFlight,
            m_MaximumRangeTrajectoryPoints);
    }

    private void DrawTrajectoryWithLabel(Vector3[] trajectoryPoints, string label, Color color, float thickness = 0.5f)
    {
        Handles.color = color;
        
        Handles.Label(trajectoryPoints[trajectoryPoints.Length / 2], label);
        
        for (var i = 1; i < trajectoryPoints.Length; i++)
        {
            Handles.DrawLine(
                trajectoryPoints[i - 1], trajectoryPoints[i], thickness);
        }
    }
}
