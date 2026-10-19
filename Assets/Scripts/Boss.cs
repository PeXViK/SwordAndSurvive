using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Boss : Entity
{
    [SerializeField] private float damage;
    [SerializeField] private float damageCooldown;
    [SerializeField] private float bulletCooldown;
    [SerializeField] private float perBulletCooldown;
    [SerializeField] private float attackRange;
    [SerializeField] private float expDrop;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Sprite[] bossSprites;
    [SerializeField] private Animator animator;
    [SerializeField] private ItemData[] drop;
    [SerializeField] private Collider2D meleeAttackTrigger;

    [SerializeField] private GameObject attackWavePrefab;

    [SerializeField] private int dimensionLvl;

    private GameObject attackWave;
    private float facingDirection = 1;
    private SpriteRenderer spriteRenderer;
    private PlayerCore player;
    private float damageTimer = 0f;
    private float bulletTimer = 0f;
    private Vector2 direction;
    private Vector2 avoidanceDirection = Vector2.zero;
    private static bool isAlive;
    private static int bossLevel = 3;
    private bool isInCollider = false;
    private bool isPlayerAttacked = false;

    public static bool IsAlive { get => isAlive; set => isAlive = value; }
    public static int BossLevel { get => bossLevel; set => bossLevel = value; }
    public float Damage { get => damage; private set => damage = value; } 
    public float ExpDrop { get => expDrop;  private set => expDrop = value; } 

    void Start()
    {
        MaxHealth = 200f * (bossLevel + 1);
        damage = 15f + bossLevel * 5;
        expDrop = 50f * (bossLevel + 1);
        CurrentHealth = MaxHealth;
        MoveSpeed = 4f;
        bulletCooldown = 5f;
        perBulletCooldown = 0.1f;
        attackRange = 0.7f;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        animator.SetInteger("bossLevel", BossLevel);
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) {
            player = playerObj.GetComponent<PlayerCore>();
        }
        isAlive = true;
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

        if (bulletTimer > 0f)
        {
            bulletTimer -= Time.deltaTime;
        }

        if (bossLevel >= 3)
        {
            if (Vector3.Distance(player.transform.position, transform.position) > 3)
            {
                BulletAttack(damage);
            }
        }
        else BulletAttack(damage);


        if (direction.x > 0 && facingDirection == -1 || direction.x < 0 && facingDirection == 1)
        {
            Flip();
        }

        if (CurrentHealth <= 0)
        {
            Die();
        }

        if (player != null && bossLevel >= 3 && Vector3.Distance(player.transform.position, transform.position) < 3)
        {
            Attack(damage);
        }
    }

    private void FixedUpdate()
    {
        if (player != null && Vector2.Distance(RigidBody.position, player.transform.position) > 0.3f && bossLevel < 3)
        {
            Move();
        }
    }

    private void Attack(float damage)
    {
        if (damageTimer <= 0f && player != null && bossLevel < 3)
        {
            player.health.TakeDamage(damage);
            damageTimer = damageCooldown;
        }

        if (damageTimer <= 0f && player != null && bossLevel >= 3)
        {
            Debug.Log("Вызов Attack из босса: " + bossLevel);
            animator.Play("melee_attack");
            damageTimer = damageCooldown;
        }
    }

    public void DoingAttack()
    {
        if (player != null && isInCollider && !isPlayerAttacked)
        {
            Debug.Log("Вызов DA из Boss");
            player.health.TakeDamage(damage);
            isPlayerAttacked = true;
            isPlayerAttacked = false;
        }
    }

    public void spawnWave()
    {
        if (attackWavePrefab != null)
        {
            attackWave = Instantiate(attackWavePrefab, transform.position, Quaternion.identity);
            attackWave.transform.parent = transform;
            Destroy(attackWave, 0.1f);
        }
    }

    private void BulletAttack(float damage)
    {
        if (bulletTimer <= 0f && player != null && bossLevel < 3)
        {
            StartCoroutine(ShootBullet());
            bulletTimer = bulletCooldown;
        }

        if (bulletTimer <= 0f && player != null && bossLevel >= 3)
        {
            animator.Play("distance_attack");
        }
    }

    private void BulletAttackHelper() {
        StartCoroutine(ShootBullet());
    }

    private void TimerReset()
    {
        damageTimer = damageCooldown;
        bulletTimer = bulletCooldown;
        Debug.Log("timer reset");
        isPlayerAttacked = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Player") && bossLevel < 3)
        {
            Attack(damage);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Временно убрано
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Игрок вошёл в зону!");
            isInCollider = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Игрок вышел из зоны!");
            isInCollider = false;
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
        float checkDistance = 2f;

        return Physics2D.Raycast(
            transform.position,
            dir,
            checkDistance,
            LayerMask.GetMask("Barrier")
        ).collider == null;
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

    private IEnumerator Knockback(Vector3 to)
    {
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

        for (int i = 0; i < 360; i += 45)
        {
            float bulletSpeed = 7f;
            float bulletLifeTime = 3f;

            if (bulletPrefab != null && player != null)
            { 
                GameObject bullet = Instantiate(bulletPrefab, gameObject.transform.position, Quaternion.identity);

                BreadBulletProjectile bl = bullet.GetComponent<BreadBulletProjectile>();
                bl.Init(dir, damage, bulletLifeTime, bulletSpeed);
                yield return new WaitForSeconds(perBulletCooldown);

                dir = Rotate(dir, 45);
            }
        }
    }
    override public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;

        if (CurrentHealth < 0)
        {
            Die();
        }

        StartCoroutine(Knockback(RigidBody.position - direction * (damage / 100)));
        
    }

    private void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    protected override void Die()
    {
        Debug.Log("Boss killed");
        player.progression.IncreaseExperience(expDrop);
        isAlive = false;
        if (bossLevel < 3)
        {
            Instantiate(
                    drop[bossLevel].prefab,
                    transform.position,
                    Quaternion.identity
                ); 
        }

        bossLevel += 1;


        Destroy(gameObject);
    }

}
