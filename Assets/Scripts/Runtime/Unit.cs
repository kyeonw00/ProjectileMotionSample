using UnityEngine;

public interface IBehaviourHandler { }

public class Unit : MonoBehaviour
{
    [Header("[Unit]")]
    [SerializeField] protected string unitName;
    [SerializeField] protected UnitStat stat;
    [SerializeField] protected MonoBehaviour behaviourHandler;

    public bool IsAlive => stat.IsAlive;

    public void TakeDamage(float damage)
    {
        stat.TakeDamage(damage);
    }
}
