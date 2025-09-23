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

public class MortarProjectile : MonoBehaviour
{
    private static readonly Collider[] HitColliders = new Collider[12];
    
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private new Collider collider;
    [SerializeField] private GameObject rendererParent;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float explosionRadius;

    private float m_GravityScale = 1f;
    
    public void Setup(Vector3 initialVelocity, float gravityScale)
    {
        rigidbody.velocity = initialVelocity;
        
        m_GravityScale = gravityScale;
    }

    private void FixedUpdate()
    {
        if (rigidbody.isKinematic)
            return;
        
        rigidbody.AddForce(Vector3.down * m_GravityScale, ForceMode.Acceleration);
    }

    private void Update()
    {
        if (rigidbody.isKinematic)
            return;
        
        transform.rotation = Quaternion.Euler(90, 0, 0) * Quaternion.LookRotation(rigidbody.velocity);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!ContainsMask(other.gameObject.layer, enemyLayer)) return;
        
        rigidbody.isKinematic = true;
        collider.enabled = false;
        rendererParent.SetActive(false);
        
        var hitCount = Physics.OverlapSphereNonAlloc(
            transform.position, explosionRadius, HitColliders, enemyLayer, QueryTriggerInteraction.Ignore);
        for (var i = 0; i < hitCount; i++)
        {
            if (!HitColliders[i].TryGetComponent(out Unit unit))
                continue;
            
            unit.TakeDamage(0f);
        }
        
        Destroy(gameObject, 3f);
    }

    private static bool ContainsMask(LayerMask container, LayerMask containee)
    {
        return ((1 << container) & containee) > 0;
    }
}
