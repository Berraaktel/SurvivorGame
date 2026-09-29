using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public float damageCooldown = 0.5f;

    private int currentHealth;
    private float lastDamageTime = -999f;

    public int CurrentHealth { get { return currentHealth; } }
    public bool IsDead { get { return currentHealth <= 0; } }

    public event Action<int, int> OnHealthChanged;
    public event Action OnDied;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Start()
    {
        if (OnHealthChanged != null) OnHealthChanged(currentHealth, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        if (Time.time - lastDamageTime < damageCooldown) return;
        lastDamageTime = Time.time;

        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        Debug.Log("Player health: " + currentHealth);

        if (OnHealthChanged != null) OnHealthChanged(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        if (OnHealthChanged != null) OnHealthChanged(currentHealth, maxHealth);
    }

    void Die()
    {
        Debug.Log("Player died!");
        if (OnDied != null) OnDied();
        gameObject.SetActive(false);
    }
}
