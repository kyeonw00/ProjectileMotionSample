/*
 * -----------------------------------------------------------------------------
 * Portfolio Rewritten Code
 *
 * 이 코드는 Buff Studio Inc.에서 작성된 원본 코드를
 * 기반으로 개인 포트폴리오 용도로 재작성한 것입니다.
 * 
 * 본 코드에는 기밀 정보가 포함되어 있지 않으며,
원본 프로젝트와는 별개로 동작합니다.
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
    private void OnSceneGUI()
    {
        var mortarSentry = (MortarSentry)target;
        var transform = mortarSentry.transform;

        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, transform.up, mortarSentry.AttackRange);
        Handles.DrawWireDisc(transform.position, transform.up, mortarSentry.MinAttackRange);

        Handles.color = Color.blue;
        Handles.DrawLine(transform.position, transform.position + mortarSentry.LaunchVelocity, 0.5f);

        Handles.color = Color.red;
        Handles.DrawLine(
            mortarSentry.BarrelTransform.position,
            mortarSentry.BarrelTransform.position + mortarSentry.BarrelTransform.transform.up * 3f,
            0.5f);
    }
}
