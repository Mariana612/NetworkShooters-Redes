using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float jumpForce = 10f;
    public int maxJumps = 2;               // 2 = doble salto

    public GroundCheck groundCheck;        // arrastra aquí el objeto con el script GroundCheck

    [Header("Disparo")]
    public GameObject bulletPrefab;
    public Transform firePoint;            // objeto vacío hijo, delante del personaje
    public float fireRate = 0.3f;          // segundos entre disparos

    [Header("Triple disparo")]
    public float tripleAngle = 10f;     // grados de apertura arriba/abajo

    private bool tripleActive;
    private Coroutine tripleRoutine;

    private float rangeMultiplier = 1f;
    private float damageMultiplier = 1f;
    private Coroutine rangeRoutine;
    private Coroutine damageRoutine;


    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private int jumpsLeft;
    private float nextFireTime;

    void Start()
    {
        jumpsLeft = maxJumps;
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float horizontal = 0;

        if (Keyboard.current.aKey.isPressed)
            horizontal = -1;

        if (Keyboard.current.dKey.isPressed)
            horizontal = 1;

        // Voltear el personaje
        if (horizontal < 0)
            spriteRenderer.flipX = true;
        else if (horizontal > 0)
            spriteRenderer.flipX = false;

        // Animación
        animator.SetFloat("movimiento", Mathf.Abs(horizontal));
        animator.SetBool("onGround", groundCheck.IsGrounded);
        animator.SetFloat("velocidadY", rb.linearVelocity.y);

        // Resetear saltos al tocar el suelo (solo si no va subiendo)
        if (groundCheck.IsGrounded && rb.linearVelocity.y <= 0.1f)
            jumpsLeft = maxJumps;

        // Saltar
        if (Keyboard.current.wKey.wasPressedThisFrame && jumpsLeft > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpsLeft--;
        }

        // Disparar
        if (Keyboard.current.spaceKey.wasPressedThisFrame && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            animator.SetTrigger("disparo");

            float direction = spriteRenderer.flipX ? -1f : 1f;

            Vector3 offset = new Vector3(Mathf.Abs(firePoint.localPosition.x) * direction,
                                         firePoint.localPosition.y, 0f);

            Vector3 spawnPos = transform.position + offset;

            SpawnBullet(spawnPos, direction, 0f);

            if (tripleActive)
            {
                SpawnBullet(spawnPos, direction, tripleAngle);
                SpawnBullet(spawnPos, direction, -tripleAngle);
            }
        }

        // Movimiento horizontal (con física)
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
    }

    public void SetColor(Color color)
    {
        color.a = 1f;
        spriteRenderer.color = color;
    }

    public void ActivatePowerUp(PowerUpType type, float duration, float multiplier)
    {
        switch (type)
        {
            case PowerUpType.Largo:
                if (rangeRoutine != null) StopCoroutine(rangeRoutine);
                rangeRoutine = StartCoroutine(RangeRoutine(duration, multiplier));
                break;

            case PowerUpType.Incrementar:
                if (damageRoutine != null) StopCoroutine(damageRoutine);
                damageRoutine = StartCoroutine(DamageRoutine(duration, multiplier));
                break;

            case PowerUpType.Triple:
                if (tripleRoutine != null) StopCoroutine(tripleRoutine);
                tripleRoutine = StartCoroutine(TripleRoutine(duration));
                break;
        }
    }

    void SpawnBullet(Vector3 position, float direction, float angle)
    {
        GameObject bullet = Instantiate(bulletPrefab, position, Quaternion.identity);

        Bullet b = bullet.GetComponent<Bullet>();
        b.lifeTime *= rangeMultiplier;
        b.damage *= damageMultiplier;
        b.Launch(direction, gameObject, rangeMultiplier > 1f, damageMultiplier > 1f, angle);
    }

    IEnumerator RangeRoutine(float duration, float multiplier)
    {
        rangeMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        rangeMultiplier = 1f;
        rangeRoutine = null;
    }

    IEnumerator DamageRoutine(float duration, float multiplier)
    {
        damageMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        damageMultiplier = 1f;
        damageRoutine = null;
    }

    IEnumerator TripleRoutine(float duration)
    {
        tripleActive = true;
        yield return new WaitForSeconds(duration);
        tripleActive = false;
        tripleRoutine = null;
    }
}
