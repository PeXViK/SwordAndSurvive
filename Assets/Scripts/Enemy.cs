using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : Entity
{
    [SerializeField] private float damage;
    [SerializeField] private float damageCooldown;
    [SerializeField] private float attackRange;
    [SerializeField] private float expDrop;

    private float facingDirection = 1;
    private SpriteRenderer spriteRenderer;
    private Player player;
    private float damageTimer = 0f;
    private Vector2 direction;
    private Vector2 avoidanceDirection = Vector2.zero;
    private Animator animator;

    public float Damage { get => damage; private set => damage = value; } 
    public float ExpDrop { get => expDrop;  private set => expDrop = value; } 

    void Start()
    {
        MaxHealth = 100f;
        CurrentHealth = MaxHealth;
        MoveSpeed = 3f;
        damage = 5f;
        damageCooldown = 1f;
        attackRange = 0.7f;
        expDrop = 1f;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) {
            player = playerObj.GetComponent<Player>();
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

        /* if (Vector2.Distance(RigidBody.position, player.transform.position) < attackRange)
        {
            Attack(damage);
        } */ // ѕеределано на соприкосновение коллайдеров

        if (CurrentHealth <= 0)
        {
            Die();
        }
        
    }

    private void FixedUpdate()
    {
        if (player != null && Vector2.Distance(RigidBody.position, player.transform.position) > 0.3f)
        {
            Move();
        }
    }

    private void Attack(float damage)
    {
        if (damageTimer <= 0f && player != null)
        {
            animator.Play("EnemyAttack");
            player.TakeDamage(damage);
            damageTimer = damageCooldown;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            Attack(damage);
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

    override public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;

        StartCoroutine(Knockback(RigidBody.position - direction * (damage / 15)));

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
        player.IncreaseExperience(expDrop);
        Destroy(gameObject);
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
}
