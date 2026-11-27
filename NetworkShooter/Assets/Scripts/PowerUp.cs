using UnityEngine;

public enum PowerUpType { Largo, Incrementar, Triple }

public class PowerUp : MonoBehaviour
{
    public PowerUpType type;
    public float duration = 4f;
    public float multiplier = 2f;          // Largo: alcance x2 / Incrementar: daño x2
    public float explosionDuration = 0.4f;

    private Animator animator;
    private Collider2D col;
    private bool collected;

    void Awake()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        if (other.TryGetComponent(out PlayerControl player))
        {
            collected = true;
            col.enabled = false;

            player.ActivatePowerUp(type, duration, multiplier);

            animator.SetTrigger("explotar");
            Destroy(gameObject, explosionDuration);
        }
    }
}