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

        if (mortarSentry.CurrentTarget != null)
        {
            Handles.color = Color.red;
            Handles.DrawLine(transform.position, mortarSentry.CurrentTarget.transform.position, 0.25f);
            Handles.DrawLine(transform.position, transform.position + mortarSentry.LaunchVelocity, 0.25f);
        }
    }
}
