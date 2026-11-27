using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 2f;
    public float damage = 10f;
    public float explosionDuration = 0.4f;   // duración del clip de explosión

    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D col;
    private GameObject owner;
    private bool exploded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    public void Launch(float direction, GameObject shooter, bool isLong = false, bool isBoosted = false, float angle = 0f)
    {
        owner = shooter;

        float rad = angle * Mathf.Deg2Rad;
        rb.linearVelocity = new Vector2(direction * Mathf.Cos(rad), Mathf.Sin(rad)) * speed;

        if (direction < 0)
            GetComponent<SpriteRenderer>().flipX = true;

        // Rota el sprite para que apunte hacia donde va
        transform.rotation = Quaternion.Euler(0f, 0f, direction * angle);

        animator.SetBool("largo", isLong);
        animator.SetBool("damage", isBoosted);

        Invoke(nameof(Explode), lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (exploded) return;

        // Ignorar al jugador que disparó
        if (other.gameObject == owner || other.transform.root.gameObject == owner)
            return;

        // Ignorar otras balas
        if (other.GetComponent<Bullet>() != null)
            return;

        if (other.GetComponent<PowerUp>() != null)
            return;

        // Aplicar daño
        if (other.TryGetComponent(out Health health))
            health.TakeDamage(damage);

        Explode();
    }

    void Explode()
    {
        exploded = true;

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        col.enabled = false;

        // Desplaza la explosión (se invierte la X si va hacia la izquierda)
        float dir = GetComponent<SpriteRenderer>().flipX ? -1f : 1f;
        transform.position += new Vector3(0.5f * dir, 0.3f, 0f);

        animator.SetTrigger("explotar");
        CancelInvoke();
        Destroy(gameObject, explosionDuration);
    }
}