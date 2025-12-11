using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private float damageAmount = 25f;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float invulnerabilityDuration = 1f;

    private float lastDamageTime;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.time - lastDamageTime < invulnerabilityDuration)
            return;

        var damageable = other.GetComponent<IDamageable>();
        if (damageable == null)
        {
            damageable = other.GetComponentInParent<IDamageable>();
        }
        if (damageable == null)
        {
            damageable = other.GetComponentInChildren<IDamageable>();
        }

        if (damageable != null)
        {
            lastDamageTime = Time.time;
            damageable.TakeDamage(damageAmount);

            if (respawnPoint != null)
            {
                other.transform.root.position = respawnPoint.position;
            }

            var rb = other.GetComponentInParent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
}
