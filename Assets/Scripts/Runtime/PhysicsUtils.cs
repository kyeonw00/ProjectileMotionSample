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

using UnityEngine;

/// <summary>
/// 프로젝트 내에서 사용되는 물리 관련 유틸리티 함수 컬렉션
/// </summary>
public static class PhysicsUtils
{
    /// <summary>
    /// Find the initial velocity of projectile to reach at Destination from Origin.
    /// </summary>
    /// <param name="origin">Origin position of projectile motion.</param>
    /// <param name="destination">Destination position of projectile motion.</param>
    /// <param name="gravity">Scale of gravity in Y-Axis.</param>
    /// <param name="timeOfFlight">Total time for the projectile remains in the air.</param>
    /// <param name="initialVelocity">Initial velocity so that projectile reach out to destination.</param>
    public static bool TryFindProjectileInitialVelocity(
        Vector3 origin, Vector3 destination, float gravity, float timeOfFlight,
        out Vector3 launchDirection, out Vector3 initialVelocity)
    {
        launchDirection = Vector3.zero;
        initialVelocity = Vector3.zero;

        var displacement = destination - origin;
        var displacementXZ = new Vector3(displacement.x, 0f, displacement.z);

        var horizontalVelocity = displacementXZ.magnitude / timeOfFlight;
        var verticalVelocity = (displacement.y + 0.5f * gravity * (timeOfFlight * timeOfFlight)) / timeOfFlight;
        var velocity = Mathf.Sqrt(horizontalVelocity * horizontalVelocity + verticalVelocity * verticalVelocity);

        var pitch = Mathf.Atan2(verticalVelocity, horizontalVelocity);
        var yaw = Mathf.Atan2(displacement.z, displacement.x);

        launchDirection =
            new Vector3(Mathf.Cos(yaw), 0f, Mathf.Sin(yaw)) * Mathf.Cos(pitch) + Vector3.up * Mathf.Sin(pitch);
        initialVelocity = launchDirection * velocity;

        return true;
    }
}