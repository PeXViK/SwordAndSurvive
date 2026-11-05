using UnityEngine;
using System.Collections;

public enum CombatType
{
    Ranged,
    Melee  
}

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : Entity
{
    [SerializeField] private float damage;
    [SerializeField] private float damageCooldown;
    [SerializeField] private float attackRange;
    [SerializeField] private float expDrop;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private CombatType combatType;
    [SerializeField] private ParticleSystem damageParticles;
    private float perBulletCooldown = 0.25f;
    

    private float facingDirection = 1;
    private SpriteRenderer spriteRenderer;
    private PlayerCore player;
    private float damageTimer = 0f;
    private Vector2 direction;
    private Vector2 avoidanceDirection = Vector2.zero;
    private Animator animator;

    public float Damage { get => damage; private set => damage = value; } 
    public float ExpDrop { get => expDrop;  private set => expDrop = value; } 

    void Start()
    {
        CurrentHealth = MaxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) {
            player = playerObj.GetComponent<PlayerCore>();
        }
    }
    

    void Update()
    {
        if (player != null)
        {
            direction = (player.transform.position - transform.position).normalized;
        }

        if (damageTimer > 0f)
        {
            damageTimer -= Time.deltaTime;
        }

        if (direction.x > 0 && facingDirection == -1 || direction.x < 0 && facingDirection == 1)
        {
            Flip();
        }

        if (combatType == CombatType.Ranged) 
        { 
            RangedAttack(damage); 
        }

        if (CurrentHealth <= 0)
        {
            Die();
        }
        
    }

    private void FixedUpdate()
    {
        if (player != null && Vector2.Distance(RigidBody.position, player.transform.position) > attackRange)
        {
            Move();
        }
    }

    private void MeleeAttack(float damage)
    {
        if (damageTimer <= 0f && player != null)
        {
            if (animator == null)
            {
                animator = gameObject.GetComponentInChildren<Animator>();
            }
            animator.Play("EnemyAttack");
            // Debug.Log("ֲûחמג Attack טח Enemy");
            damageTimer = damageCooldown;
        }
    }

    private void RangedAttack(float damage)
    {
        if (damageTimer <= 0f && player != null)
        {
            if (animator == null)
            {
                animator = gameObject.GetComponentInChildren<Animator>();
            }
            animator.Play("EnemyAttack");
            // Debug.Log("ֲûחמג Attack טח Enemy");
            StartCoroutine(ShootBullet());
            damageTimer = damageCooldown;
        }
    }

    public void DoingAttack()
    {
        if (player != null)
        { 
            // Debug.Log("ֲûחמג DA טח Enemy");
            player.health.TakeDamage(damage);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            if(combatType == CombatType.Melee) MeleeAttack(damage);
        }
    }

    public override void Move()
    {
        if (player == null) return;

        Vector2 toPlayer = (player.transform.position - transform.position).normalized;

       
        if (IsDirectionFree(toPlayer))
        {
            avoidanceDirection = Vector2.zero;
            direction = toPlayer;
        }
        else
        {
            
            if (avoidanceDirection != Vector2.zero && IsDirectionFree(avoidanceDirection))
            {
                direction = avoidanceDirection;
            }
            else
            {                
                Vector2 best = FindNewWay(toPlayer);

                if (best != Vector2.zero)
                {
                    avoidanceDirection = best;  
                    direction = best;
                }
                else
                {
                    return;  
                }
            }
        }

        RigidBody.MovePosition(
            RigidBody.position + direction * MoveSpeed * Time.fixedDeltaTime
        );
    }

    private bool IsDirectionFree(Vector2 dir)
    {
        float checkDistance = 1f;
        return Physics2D.Raycast(transform.position, dir, checkDistance, LayerMask.GetMask("Barrier")).collider == null;
    }

    private Vector2 FindNewWay(Vector2 desiredDirection)
    {
        int rays = 24;               
        float maxAngle = 180f;       
        float angleStep = maxAngle * 2 / rays;

        float bestDot = -1f;
        Vector2 bestDir = Vector2.zero;

        for (int i = 0; i < rays; i++)
        {
            float angle = -maxAngle + angleStep * i;
            Vector2 dir = Rotate(desiredDirection, angle);

            if (IsDirectionFree(dir))
            {
                float dot = Vector2.Dot(desiredDirection, dir);
                if (dot > bestDot)
                {
                    bestDot = dot;
                    bestDir = dir;
                }
            }
        }

        return bestDir; 
    }

    private Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);

        return new Vector2(
            cos * v.x - sin * v.y,
            sin * v.x + cos * v.y
        );
    }

    override public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;

        damageParticles.Play();
        StartCoroutine(Knockback(RigidBody.position - direction * (damage / 50)));

        if (CurrentHealth < 0)
        {
            Die();
        }
    }

    private void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    protected override void Die()
    {
        Debug.Log("Enemy killed");
        player.progression.IncreaseExperience(expDrop);
        Destroy(gameObject);
    }

    private IEnumerator Knockback(Vector3 to)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        float elapsed = 0f;
        float duration = 0.1f;

        Vector3 from = transform.position;
        spriteRenderer.color = Color.red;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = Color.white;
    }

    private IEnumerator ShootBullet()
    {
        Vector2 dir = (player.transform.position - transform.position).normalized;

        for (int i = 0; i < 5; i++)
        {
            float bulletSpeed = 7f;
            float bulletLifeTime = 1.5f;

            if (bulletPrefab != null && player != null)
            {
                GameObject bullet = Instantiate(bulletPrefab, gameObject.transform.position, Quaternion.identity);

                BreadBulletProjectile bl = bullet.GetComponent<BreadBulletProjectile>();
                bl.Init(dir, damage, bulletLifeTime, bulletSpeed);
                yield return new WaitForSeconds(perBulletCooldown);

                dir = (player.transform.position - transform.position).normalized;
            }
        }
    }
}
