using UnityEngine;
using System;
using System.Collections;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float deathDuration = 1f;     // duración del clip de muerte

    public float CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    public event Action<float, float> OnHealthChanged;
    public event Action OnDied;

    void Start()
    {
        if (HUDManager.Instance != null)
            HUDManager.Instance.RegisterPlayer(this);
    }

    void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth <= 0f)
            StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        IsDead = true;
        OnDied?.Invoke();

        // Desactiva control y física
        if (TryGetComponent(out PlayerControl control))
            control.enabled = false;

        if (TryGetComponent(out Rigidbody2D rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        if (TryGetComponent(out Animator animator))
            animator.SetTrigger("morir");

        yield return new WaitForSeconds(deathDuration);

        gameObject.SetActive(false);
    }
}