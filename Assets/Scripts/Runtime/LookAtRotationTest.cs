using System;
using UnityEngine;

public class LookAtRotationTest : MonoBehaviour
{
    public Transform targetToFollow;
    public Transform yawTarget;
    public Transform pitchTarget;
    public float apexTime;

    private void Update()
    {
        if (targetToFollow == null)
            return;
        
        // update yaw towards target

        var displacement = targetToFollow.position - transform.position;
        var distanceXZ = DistanceInXZCoord(transform.position, targetToFollow.position);
        var direction = displacement.normalized;

        var yawRotation = Quaternion.LookRotation(direction, yawTarget.up);
        var yawEulerAngles = yawRotation.eulerAngles;
        yawRotation = Quaternion.Euler(0, yawEulerAngles.y, 0);

        yawTarget.rotation = yawRotation;
        
        // update pitch towards target

        var horizontalVelocity = distanceXZ / apexTime;
        var verticalVelocity = (displacement.y + 0.5f * Mathf.Abs(Physics.gravity.y) * apexTime * apexTime) / apexTime;
        var velocity = Mathf.Sqrt(horizontalVelocity * horizontalVelocity + verticalVelocity * verticalVelocity);
        var cosTheta = (velocity > 0f) ? horizontalVelocity / velocity : 1f;
        var sinTheta = (velocity > 0f) ? verticalVelocity / velocity : 1f;
        cosTheta = Mathf.Clamp(cosTheta, -1f, 1f);
        sinTheta = Mathf.Clamp(sinTheta, -1f, 1f);

        var pitch = Mathf.Atan2(sinTheta, cosTheta);
    }

    private static float DistanceInXZCoord(Vector3 a, Vector3 b)
    {
        var displacement = a - b;
        displacement.y = 0f; // reset y diff to only calculate in XZ coord
        return displacement.magnitude;
    }
}
