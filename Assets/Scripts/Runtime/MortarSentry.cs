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

using UnityEngine;

public class MortarSentry : MonoBehaviour
{
    private const float MinimumPitchDegree = 0f;
    private const float MaximumPitchDegree = 44f;
    
    [Header("[Mortar Sentry]")]
    [SerializeField] private float attackRange; // 최대 공격 사거리
    [SerializeField] private float minAttackRange; // 최소 공격 사거리
    [SerializeField] private float fireRate; // 분당 공격 횟수
    [SerializeField] private Transform bodyTransform; // 센트리 몸통 트랜스폼
    [SerializeField] private Transform barrelTransform; // 센트리 포신 트랜스폼
    [SerializeField] private float sentryRotateSpeed; // 센트리 회전 속도 (클수록 빠르게 회전)

    [Space]
    [SerializeField] private MortarProjectile projectilePrefab; // 투사체 프리팹
    [SerializeField] private float projectileTimeOfFlight; // 투사체 체공 시간
    [SerializeField] private float projectileGravity = 9.82f; // 투사체 중력 가중치

    [Space]
    [SerializeField] private Collider currentTarget;

    private float m_LastFireTime = -1f;
    private Quaternion m_BarrelBaseLocalRotation;
    private float m_BodyYawDeg;
    private float m_BarrelPitchDeg;
    
    public float AttackRange => attackRange;
    public float MinAttackRange => minAttackRange;
    public float ProjectileGravity => projectileGravity;
    public float ProjectileTimeOfFlight => projectileTimeOfFlight;
    public Vector3 LaunchVelocity { get; private set; }
    public Transform BarrelTransform => barrelTransform;

    private void Start()
    {
        m_BarrelBaseLocalRotation = barrelTransform.localRotation;
        m_BarrelPitchDeg = 0f;
        m_BodyYawDeg = bodyTransform.eulerAngles.y;
    }

    private void Update()
    {
        if (currentTarget == null)
            return;
        
        var launchPoint = transform.position; // 발사 위치
        var impactPoint = currentTarget.transform.position; // 목표 위치
        
        // 포탄 초기 속도/방향 계산
        PhysicsUtils.TryFindProjectileInitialVelocity(
            launchPoint, impactPoint, projectileGravity, projectileTimeOfFlight,
            out var launchDirection, out var launchVelocity);

        LaunchVelocity = launchDirection * launchVelocity;
        
        // 포탄 발사
        FireProjectile(launchPoint, LaunchVelocity);

        // 포신 및 포열의 방향 발사각에 맞도록 컨트롤
        var rotateDelta = sentryRotateSpeed * Time.deltaTime;
        var directionXZ = new Vector3(launchDirection.x, 0f, launchDirection.z);
        
        // 포신은 Yaw(Y-Axis)만 회전
        var targetYawDeg = Mathf.Rad2Deg * Mathf.Atan2(directionXZ.x, directionXZ.z);
        m_BodyYawDeg = Mathf.MoveTowardsAngle(m_BodyYawDeg, targetYawDeg, rotateDelta);
        bodyTransform.rotation = Quaternion.AngleAxis(m_BodyYawDeg, Vector3.up);;
        
        // 포열은 Pitch(X-Axis)만 회전
        var localLaunchDir = bodyTransform.InverseTransformDirection(launchDirection).normalized;
        var localInBase = Quaternion.Inverse(m_BarrelBaseLocalRotation) * localLaunchDir;
        var targetPitchDeg = Mathf.Rad2Deg * Mathf.Atan2(localInBase.z, localInBase.y);
        targetPitchDeg = Mathf.Clamp(targetPitchDeg, MinimumPitchDegree, MaximumPitchDegree);
        m_BarrelPitchDeg = Mathf.MoveTowardsAngle(m_BarrelPitchDeg, targetPitchDeg, rotateDelta);
        barrelTransform.localRotation = m_BarrelBaseLocalRotation * Quaternion.AngleAxis(m_BarrelPitchDeg, Vector3.right);
    }

    private void FireProjectile(Vector3 launchPoint, Vector3 launchVelocity)
    {
        if (m_LastFireTime + (60f / fireRate) > Time.time)
            return;
        
        m_LastFireTime = Time.time;

        var projectile = Instantiate(projectilePrefab, launchPoint, Quaternion.identity, null);
        projectile.Setup(launchVelocity, projectileGravity);
    }
}
