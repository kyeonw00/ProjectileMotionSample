using UnityEngine;

public class UnitStat : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private float attackDamage;
    [SerializeField] private float defence;
    [SerializeField] private float elementalDefense;
    [SerializeField] private int installCost;
    [SerializeField] private float attackSpeed;

    [Space]
    [SerializeField] private int currentHealth;

    public bool IsAlive => currentHealth > 0;

    public void Setup()
    {
        currentHealth = health;
    }

    /// <summary>Take amount of given damage</summary>
    /// <param name="damage">Damage amount to take</param>
    /// <returns>returns TRUE if unit is still alive after taking damage</returns>
    public void TakeDamage(float damage)
    {
        var reducedDamage = damage - defence;
        if (reducedDamage < 0)
            reducedDamage = 0f;
        
        var hp = currentHealth - Mathf.FloorToInt(reducedDamage);
        if (hp < 0)
            hp = 0;

        currentHealth = hp;
    }

    public float GetAttackDamage()
    {
        return attackDamage;
    }
}
