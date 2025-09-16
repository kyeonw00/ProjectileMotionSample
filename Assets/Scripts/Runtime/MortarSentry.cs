using System.Collections;
using UnityEngine;

public class MortarSentry : Unit
{
    [Header("[Mortar Sentry]")]
    [SerializeField] private float attackRange;
    [SerializeField] private float minAttackRange;
    [SerializeField] private float fireRate; // rate per minute
    [SerializeField] private float enemySearchInterval;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float barrelRotateSpeed;
    [SerializeField] private float projectileApexTime;
    [SerializeField] private float gravityScale;

    [Space]
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Transform muzzleTransform;
    [SerializeField] private MortarProjectile projectilePrefab;

    private float m_GravityConstant;
    private float m_SqrProjectileApexTime;
    private float m_ProjectileApexTime;
    private float m_VerticalVelocityDeterminer;
    private Vector3 m_LaunchDirection;
    private Vector3 m_LaunchVelocity;
    private float m_LastFireTime = -1f;
    
    private Coroutine m_DetectEnemyCoroutine;
    private Collider m_CurrentTarget;
    private Coroutine m_FireProjectileCoroutine;

    private readonly Collider[] m_SearchResults = new Collider[8];

    public bool IsAlive { get; set; }

    public float AttackRange => attackRange;

    public float MinAttackRange => minAttackRange;
    
    public Collider CurrentTarget => m_CurrentTarget;

    public Vector3 LaunchVelocity => m_LaunchVelocity;

    private void Start()
    {
        IsAlive = true;
        
        m_GravityConstant = Mathf.Abs(Physics.gravity.y) * gravityScale;
        m_ProjectileApexTime = projectileApexTime;
        m_SqrProjectileApexTime = projectileApexTime * projectileApexTime;
        m_VerticalVelocityDeterminer = 0.5f * m_GravityConstant * m_SqrProjectileApexTime;
        
        m_DetectEnemyCoroutine = StartCoroutine(DetectEnemy());
    }

    private void Update()
    {
        if (m_CurrentTarget != null)
        {
            var direction = new Vector3(
                m_CurrentTarget.transform.position.x - transform.position.x, 0f, m_CurrentTarget.transform.position.z - transform.position.z);
            var targetRotation = Quaternion.LookRotation(direction, Vector3.up);

            bodyTransform.rotation =
                Quaternion.RotateTowards(bodyTransform.rotation, targetRotation, barrelRotateSpeed * Time.deltaTime);
            
            var launchPoint = muzzleTransform.position + muzzleTransform.forward * 0.5f;
            var impactPoint = m_CurrentTarget.transform.position;
            
            var displacement = impactPoint - launchPoint;
            var distanceXZ = new Vector3(displacement.x, 0f, displacement.z).magnitude;
            var horizontalVelocity = distanceXZ / m_ProjectileApexTime;
            var verticalVelocity = (displacement.y + m_VerticalVelocityDeterminer) / m_ProjectileApexTime;
            var velocity = Mathf.Sqrt(horizontalVelocity * horizontalVelocity + verticalVelocity * verticalVelocity);

            var cosTheta = velocity > 0f ? horizontalVelocity / velocity : 1f;
            var sinTheta = velocity > 0f ? verticalVelocity / velocity : 1f;

            cosTheta = Mathf.Clamp(cosTheta, -1f, 1f);
            sinTheta = Mathf.Clamp(sinTheta, -1f, 1f);

            var pitch = Mathf.Atan2(sinTheta, cosTheta);
            var yaw = Mathf.Atan2(displacement.z, displacement.x);
            var horizontalDirection = new Vector3(Mathf.Cos(yaw), 0f, Mathf.Sin(yaw));
            var launchDirection = horizontalDirection * Mathf.Cos(pitch) + Vector3.up * Mathf.Sin(pitch);

            m_LaunchDirection = launchDirection;
            m_LaunchVelocity = launchDirection * velocity;
            
            FireProjectile(launchPoint);
        }
    }

    private IEnumerator DetectEnemy()
    {
        WaitForSeconds searchInterval = new(enemySearchInterval);
        
        while (IsAlive)
        {
            yield return searchInterval;

            if (m_CurrentTarget != null)
            {
                float distance = Vector3.Distance(transform.position, m_CurrentTarget.transform.position);
                if (distance >= minAttackRange && distance <= attackRange)
                {
                    continue;
                }
            }

            m_CurrentTarget = null;
            
            var currentDistance = attackRange * 2f;
            var position = transform.position;
            var collisionCount =
                Physics.OverlapSphereNonAlloc(position, attackRange, m_SearchResults, enemyLayer);

            for (var i = 0; i < collisionCount; i++)
            {
                var distance = Vector3.Distance(position, m_SearchResults[i].transform.position);

                if (distance < minAttackRange ||
                    distance > attackRange ||
                    distance > currentDistance) continue;

                m_CurrentTarget = m_SearchResults[i];
                currentDistance = distance;
            }
        }
    }

    private void FireProjectile(Vector3 launchPoint)
    {
        if (m_LastFireTime + (fireRate / 60f) > Time.time)
            return;
        
        m_LastFireTime = Time.time;

        var projectile = Instantiate(projectilePrefab, launchPoint, Quaternion.identity, null);
        projectile.Setup(m_LaunchVelocity, stat.GetAttackDamage(), gravityScale);
    }

    private static float DistanceInXZCoord(Vector3 a, Vector3 b)
    {
        var displacement = a - b;
        displacement.y = 0f; // reset y diff to only calculate in XZ coord
        return displacement.magnitude;
    }
}
