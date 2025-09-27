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

public class MortarSentry : MonoBehaviour
{
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

    private Quaternion m_BarrelDefaultRotation;
    private float m_LastFireTime = -1f;
    
    public float AttackRange => attackRange;
    public float MinAttackRange => minAttackRange;

    private void Start()
    {
        m_BarrelDefaultRotation = barrelTransform.localRotation;
    }

    private void Update()
    {
        if (currentTarget == null)
            return;
        
        var launchPoint = transform.position; // 발사 위치
        var impactPoint = currentTarget.transform.position; // 목표 위치
        
        // 포탄 초기속도 계산
        PhysicsUtils.TryFindProjectileInitialVelocity(
            launchPoint, impactPoint, projectileGravity, projectileTimeOfFlight,
            out var launchDirection, out var launchVelocity);

        var rotateDelta = sentryRotateSpeed * Time.deltaTime;
        
        // note:
        //  기획상 센트리의 Transform.up이 항상 Vector3.up을 바라봄
        //  별도의 로컬 변환을 고려할 필요 없음
            
        FireProjectile(launchPoint, launchVelocity);
    }

    private void FireProjectile(Vector3 launchPoint, Vector3 launchVelocity)
    {
        if (m_LastFireTime + (fireRate / 60f) > Time.time)
            return;
        
        m_LastFireTime = Time.time;

        var projectile = Instantiate(projectilePrefab, launchPoint, Quaternion.identity, null);
        projectile.Setup(launchVelocity, projectileGravity);
    }
}
