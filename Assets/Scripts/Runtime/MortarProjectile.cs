using UnityEngine;

public class MortarProjectile : MonoBehaviour
{
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private new Collider collider;
    [SerializeField] private GameObject rendererParent;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float explosionRadius;

    private float m_DamageContext;
    private float m_GravityScale = 1f;

    public void Setup(Vector3 initialVelocity, float damage, float gravityScale)
    {
        rigidbody.velocity = initialVelocity;

        m_DamageContext = damage;
        m_GravityScale = gravityScale;
    }

    private void FixedUpdate()
    {
        // adjust gravity acceleration
        // note
        //  - for the physical quantity offset by unity physics, below formula used:
        //  - (gravity * -1f) + (gravity * gravity_scale) = gravity * (gravity_scale - 1)
        rigidbody.AddForce(Physics.gravity * (m_GravityScale - 1f), ForceMode.Acceleration);
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(90, 0, 0) * Quaternion.LookRotation(rigidbody.velocity);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!ContainsMask(other.gameObject.layer, enemyLayer)) return;
        
        rigidbody.isKinematic = true;
        collider.enabled = false;
        rendererParent.SetActive(false);

        var hits =
            Physics.OverlapSphere(transform.position, explosionRadius, enemyLayer, QueryTriggerInteraction.Ignore);
        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent(out Unit unit))
                continue;
            
            unit.TakeDamage(m_DamageContext);
        }
        
        Destroy(gameObject, 3f);
    }

    private static bool ContainsMask(LayerMask container, LayerMask containee)
    {
        return ((1 << container) & containee) > 0;
    }
}
