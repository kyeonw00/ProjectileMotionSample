using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class MortarSentry : Unit
{
    [SerializeField] private float attackRange;
    [SerializeField] private float minAttackRange;
    [SerializeField] private float fireRate; // rate per minute
    [SerializeField] private float enemySearchInterval;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float barrelRotateSpeed;
    [SerializeField] private float projectileApexTime;

    [Space]
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Transform muzzleTransform;
    [SerializeField] private MortarProjectile projectilePrefab;

    private float m_LastFireTime = -1f;
    
    private Coroutine m_DetectEnemyCoroutine;
    private Collider m_CurrentTarget;
    private Coroutine m_FireProjectileCoroutine;

    private readonly Collider[] m_SearchResults = new Collider[8];
    
    public bool IsAlive { get; set; }

    private void Start()
    {
        m_DetectEnemyCoroutine = StartCoroutine(DetectEnemy());
    }

    private void Update()
    {
        if (!IsAlive) return;
        
        RotateBarrel();
        FireProjectile();
    }

    private IEnumerator DetectEnemy()
    {
        WaitForSeconds searchInterval = new(enemySearchInterval);
        
        while (IsAlive)
        {
            yield return searchInterval;

            if (m_CurrentTarget == null)
                continue;
            
            var collisionCount =
                Physics.OverlapSphereNonAlloc(transform.position, attackRange, m_SearchResults, enemyLayer);

            Collider currentTarget = null;
            float currentDistance = attackRange * 2f;

            Vector3 position = transform.position;
            
            for (var i = 0; i < collisionCount; i++)
            {
                float distance = Vector3.Distance(position, m_SearchResults[i].transform.position);

                if (distance > currentDistance)
                    continue;
                
                currentTarget = m_SearchResults[i];
                currentDistance = distance;
            }
        }
    }

    private void RotateBarrel()
    {
        if (m_CurrentTarget == null) return;

        var direction = new Vector3(
            m_CurrentTarget.transform.position.x - transform.position.x, 0f, m_CurrentTarget.transform.position.z - transform.position.z);
        var targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        bodyTransform.rotation =
            Quaternion.RotateTowards(bodyTransform.rotation, targetRotation, barrelRotateSpeed * Time.deltaTime);
    }

    private void FireProjectile()
    {
        if (m_LastFireTime + (fireRate / 60f) < Time.time)
            return;
        
        m_LastFireTime = Time.time;
        
        var spawnPoint = muzzleTransform.position + muzzleTransform.forward * 0.5f;

        var position = transform.position;
        var targetPosition = m_CurrentTarget.bounds.center;
        
        // Calculate initial velocity
        
        var displacement = targetPosition - spawnPoint;
        var displacementXZ = DistanceInXZCoord(position, targetPosition);
        
        var horizontalVelocity = displacementXZ / projectileApexTime;
        var verticalVelocity =
            (displacement.y + 0.5f * Mathf.Abs(Physics.gravity.y) * projectileApexTime * projectileApexTime) /
            projectileApexTime;

        var velocity = Mathf.Sqrt(horizontalVelocity * horizontalVelocity + verticalVelocity * verticalVelocity);

        // Calculate launch angle
        
        var cosTheta = (velocity > 0f) ? horizontalVelocity / velocity : 1f;
        var sinTheta = (velocity > 0f) ? verticalVelocity / velocity : 1f;

        cosTheta = Mathf.Clamp(cosTheta, -1f, 1f);
        sinTheta = Mathf.Clamp(sinTheta, -1f, 1f);

        var pitch = Mathf.Atan2(sinTheta, cosTheta);

        var yaw = Mathf.Atan2(displacement.z, displacement.x);
        var horizontalDirection = new Vector3(Mathf.Cos(yaw), 0f, Mathf.Sin(yaw));
        var direction = horizontalDirection * Mathf.Cos(pitch) + Vector3.up * Mathf.Sin(pitch);

        var projectile = Instantiate(projectilePrefab, spawnPoint, Quaternion.identity, null);
        projectile.Setup(direction * velocity, stat.GetAttackDamage());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private static float DistanceInXZCoord(Vector3 a, Vector3 b)
    {
        var displacement = a - b;
        displacement.y = 0f; // reset y diff to only calculate in XZ coord
        return displacement.magnitude;
    }
}
