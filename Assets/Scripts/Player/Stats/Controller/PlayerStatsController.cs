using UnityEngine;
using System;

public class PlayerStatsController : MonoBehaviour, IHealthReceiver, IDamageable
{
    [SerializeField] private StatController healthController;

    public event Action OnPlayerDeath;

    public StatController Health => healthController;

    public float CurrentHealth => healthController?.CurrentValue ?? 0f;
    public float MaxHealth => healthController?.MaxValue ?? 0f;
    public bool IsFullHealth => healthController?.IsFull ?? true;
    public bool IsDead => healthController?.IsDepleted ?? false;

    public void Heal(float amount)
    {
        healthController?.Add(amount);
    }

    public void TakeDamage(float amount)
    {
        healthController?.Remove(amount);
    }

    void OnEnable()
    {
        if (healthController != null)
        {
            healthController.OnDepleted += HandleHealthDepleted;
        }
    }

    void OnDisable()
    {
        if (healthController != null)
        {
            healthController.OnDepleted -= HandleHealthDepleted;
        }
    }

    private void HandleHealthDepleted()
    {
        OnPlayerDeath?.Invoke();
    }
}
