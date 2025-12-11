using UnityEngine;
using System;

public class PlayerStatsController : MonoBehaviour, IHealthReceiver, IDamageable
{
    [SerializeField] private StatController healthController;
    [SerializeField] private GameResetController gameResetController;
    [SerializeField] private float deathResetDelay = 3f;

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
        
        DisablePlayer();
        NotificationManager.Instance?.ShowError("You are dead");
        Invoke(nameof(ResetAfterDeath), deathResetDelay);
    }

    private void DisablePlayer()
    {
        var movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.enabled = false;
        }

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        var collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }

    private void ResetAfterDeath()
    {
        if (gameResetController != null)
        {
            gameResetController.ResetGame();
        }
    }
}
