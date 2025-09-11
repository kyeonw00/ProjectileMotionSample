using UnityEngine;

public class MortarProjectile : MonoBehaviour
{
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private new Collider collider;
    [SerializeField] private GameObject rendererParent;
    [SerializeField] private ParticleSystem explosionVfx;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float explosionRadius;

    private float m_DamageContext;

    public void Setup(Vector3 initialVelocity, float damage)
    {
        rigidbody.velocity = initialVelocity;

        m_DamageContext = damage;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (!ContainsMask(other.gameObject.layer, enemyLayer)) return;
        
        rigidbody.isKinematic = true;
        collider.enabled = false;
        rendererParent.SetActive(false);
        explosionVfx.Play();

        var hits =
            Physics.OverlapSphere(transform.position, explosionRadius, enemyLayer, QueryTriggerInteraction.Ignore);
        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent(out Unit unit))
                continue;
            
            unit.TakeDamage(m_DamageContext);
        }
    }

    private static bool ContainsMask(LayerMask container, LayerMask containee)
    {
        return ((1 << container) & containee) > 0;
    }
}
