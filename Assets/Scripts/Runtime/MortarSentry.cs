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
    [SerializeField] private float attackRange; // max attack range
    [SerializeField] private float minAttackRange; // min attack range
    [SerializeField] private float fireRate; // fire rate per minute
    [SerializeField] private float projectileTimeOfFlight; // projectile's total flight of time
    
    [Space]
    [SerializeField] private float gravityScale;
    [SerializeField] private float barrelRotateSpeed;

    [Space]
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private MortarProjectile projectilePrefab;

    [Space]
    [SerializeField] private Collider currentTarget;

    private float m_LastFireTime = -1f;
    
    public float AttackRange => attackRange;
    public float MinAttackRange => minAttackRange;

    private void Update()
    {
        if (currentTarget == null)
            return;
        
        var launchPoint = transform.position;
        var impactPoint = currentTarget.transform.position;
            
        var displacement = impactPoint - launchPoint;
        var displacementXZ = new Vector3(displacement.x, 0f, displacement.z);
        var distanceXZ = displacementXZ.magnitude;
        var horizontalVelocity = distanceXZ / projectileTimeOfFlight;
        var verticalVelocity =
            (displacement.y + 0.5f * gravityScale * (projectileTimeOfFlight * projectileTimeOfFlight)) / projectileTimeOfFlight;
        var velocity = Mathf.Sqrt(horizontalVelocity * horizontalVelocity + verticalVelocity * verticalVelocity);
            
        var pitch = Mathf.Atan2(verticalVelocity, horizontalVelocity);
        var yaw = Mathf.Atan2(displacement.z, displacement.x);
        var horizontalDirection = new Vector3(Mathf.Cos(yaw), 0f, Mathf.Sin(yaw));
        var launchDirection = horizontalDirection * Mathf.Cos(pitch) + Vector3.up * Mathf.Sin(pitch);
        var launchVelocity = launchDirection * velocity;

        var rotateDelta = barrelRotateSpeed * Time.deltaTime;
        
        bodyTransform.rotation = Quaternion.RotateTowards(
            bodyTransform.rotation, Quaternion.LookRotation(displacementXZ.normalized, Vector3.up), rotateDelta);
            
        FireProjectile(launchPoint, launchVelocity);
    }

    private void FireProjectile(Vector3 launchPoint, Vector3 launchVelocity)
    {
        if (m_LastFireTime + (fireRate / 60f) > Time.time)
            return;
        
        m_LastFireTime = Time.time;

        var projectile = Instantiate(projectilePrefab, launchPoint, Quaternion.identity, null);
        projectile.Setup(launchVelocity, gravityScale);
    }
}
