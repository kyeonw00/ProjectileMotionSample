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
    /// 발사 지점 <paramref name="origin"/>으로 부터 도착 지점 <paramref name="destination"/> 까지 도달하기 위한 투사체의
    /// 초기 발사 속도 <paramref name="initialVelocity"/>를 계산합니다.
    /// </summary>
    /// <param name="origin">발사 지점</param>
    /// <param name="destination">도착 지점</param>
    /// <param name="gravity">발사체에 가해지는 중력 크기</param>
    /// <param name="timeOfFlight">투사체의 체공 시간</param>
    /// <param name="initialVelocity">초기 발사 속도</param>
    public static bool TryFindProjectileInitialVelocity(
        Vector3 origin, Vector3 destination, float gravity, float timeOfFlight,
        out Vector3 launchDirection, out Vector3 initialVelocity)
    {
        launchDirection = Vector3.zero;
        initialVelocity = Vector3.zero;

        var displacement = destination - origin;
        var displacementXZ = new Vector3(displacement.x, 0f, displacement.z);

        // 거리가 너무 가까우면 해를 구할 수 없음
        if (displacementXZ.sqrMagnitude < float.Epsilon)
            return false;
        
        var horizontalVelocity = displacementXZ.magnitude / timeOfFlight;
        var verticalVelocity = (displacement.y + 0.5f * gravity * (timeOfFlight * timeOfFlight)) / timeOfFlight;
        var velocity = Mathf.Sqrt(horizontalVelocity * horizontalVelocity + verticalVelocity * verticalVelocity);

        var pitch = Mathf.Atan2(verticalVelocity, horizontalVelocity);
        var yaw = Mathf.Atan2(displacement.z, displacement.x);

        launchDirection =
            new Vector3(Mathf.Cos(yaw), 0f, Mathf.Sin(yaw)) * Mathf.Cos(pitch) + Vector3.up * Mathf.Sin(pitch);
        launchDirection.Normalize();
        
        initialVelocity = launchDirection * velocity;

        return true;
    }
}