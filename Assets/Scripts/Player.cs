using System.Collections;
using TMPro;
using UnityEngine;
[RequireComponent (typeof(Rigidbody2D))]
public class Player : Entity
{
    [SerializeField] private int level;
    [SerializeField] private float experience;
    [SerializeField] private float expToNextLevel;
    [SerializeField] private float attackDamageMultiplier;
    [SerializeField] private float attackSpeedMultiplier;
    [SerializeField] private float attackRangeMultiplier;
    [SerializeField] private float damageResistance;
    [SerializeField] private Weapon currentWeapon;
    [SerializeField] private Weapon[] weapons;
    [SerializeField] private Animator anim;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private TextMeshProUGUI expUI;
    [SerializeField] private TextMeshProUGUI levelUI;
    [SerializeField] private GameObject simpleWave;
    [SerializeField] private GameObject longWave;
    [SerializeField] private GameObject heavyWave;
    [SerializeField] private GameObject bulletProjectile;
    [SerializeField] private GameObject shieldEffectPrefab;
    [SerializeField] private Transform attackPoint;
    public GameObject activeShield;
    private bool isBoostActive = false;
    private Rigidbody2D rb;
    private bool isDeath;
    private Vector2 moveInput;
    private float attackTimer;
    private float dashTimer;
    private Vector3 facingDirection = new Vector3(1, 1, 0);
    Vector3 attackPosition = new Vector3(0,0,0);
    private int slot = 1;
    private int bulletCount = 0;

    public Vector2 MoveInput { get => moveInput; private set => moveInput = value; }
    public Transform AttackPoint { get { return attackPoint; } }
    public float AttackDamageMultiplier { get => attackDamageMultiplier; private set => attackDamageMultiplier = value; }
    public bool IsDeath { get => isDeath; private set => isDeath = value; }
    public float AttackSpeedMultiplier { get => attackSpeedMultiplier; private set => attackSpeedMultiplier = value; }
    public float AttackRangeMultiplier { get => attackRangeMultiplier; private set => attackRangeMultiplier = value; }
    public Weapon CurrentWeapon { get => currentWeapon; private set => currentWeapon = value; }
    public Animator Animator { get =>  anim; private set => anim = value; }

    void Awake()
    {   
        isDeath = false;
        damageResistance = 0;
        MaxHealth = 100f;
        CurrentHealth = MaxHealth;
        MoveSpeed = 5f;
        level = 1;
        experience = 0;
        expToNextLevel = 10;
        attackDamageMultiplier = 1;
        attackSpeedMultiplier = 1;
        attackRangeMultiplier = 1;
        weapons[0] = gameObject.AddComponent<SimpleSword>();
        weapons[1] = gameObject.AddComponent<LongSword>();
        weapons[2] = gameObject.AddComponent<HeavySword>();
        currentWeapon = weapons[0];
        healthBar.SetActive(true);
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
     
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        anim.SetFloat("horizontal", Mathf.Abs(moveInput.x));
        anim.SetFloat("vertical", Mathf.Abs(moveInput.y));

        expUI.text = "Опыт: " + experience + "/" + expToNextLevel;
        levelUI.text = "Уровень " + level.ToString();

        if (experience >= expToNextLevel)
        {
            LevelUp();
        }


        if (Input.GetButtonDown("Jump") && dashTimer <= 0)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetMouseButton(0) && !isDeath)
        {
            StartAttack();
        }

        if (Input.GetMouseButton(1) && bulletCount > 0 && !isDeath)
        {
            BulletAttack();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) { 
            EquipWeapon(0); 
            slot = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && level >= 5) {
            EquipWeapon(1);
            slot = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && level >= 10) { 
            EquipWeapon(2);
            slot = 3;
        }

        Rotate();

        healthBar.transform.localScale = new Vector3(CurrentHealth / MaxHealth, 1, 1);

        TimerUpdate();
    
    }

    public void IncreaseExperience(float value)
    {
        experience += value;
    }

    private void LevelUp()
    {
        level += 1;
        experience -= expToNextLevel;
        expToNextLevel += level;
        attackRangeMultiplier += 0.01f;
        attackSpeedMultiplier += 0.01f;
        attackDamageMultiplier += 0.05f;
        if (CurrentHealth < MaxHealth-10)
        {
            MaxHealth += 5;
            CurrentHealth += 10;
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void StartAttack()
    {
        if (currentWeapon != null && attackTimer <= 0)
        {
            switch(slot)
            {
                case 1:
                    anim.Play("PlayerSimpleAttack");
                    break;
                case 2:
                    anim.Play("PlayerLongAttack");
                    break;
                case 3:
                    anim.Play("PlayerHeavyAttack");
                    break;
                default:
                    break;
            }  

            GameObject prefab = null;
            if (slot == 1) prefab = simpleWave;
            if (slot == 2) prefab = longWave;
            if (slot == 3) prefab = heavyWave;

            float waveLifeTime = currentWeapon.WaveLifeTime;
            float waveSpeed = currentWeapon.AttackRange / waveLifeTime;

            if (prefab != null)
            {
                Vector2 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;

                if (dir.x >= -0.1f && facingDirection.x >= 0 || dir.x <= 0.1f && facingDirection.x <= 0)
                {
                    GameObject wave = Instantiate(prefab, attackPoint.position, Quaternion.identity);
                    // Debug.Log(dir.ToString());

                    WaveProjectile wp = wave.GetComponent<WaveProjectile>();
                    wp.Init(dir, currentWeapon.BaseDamage * AttackDamageMultiplier, waveLifeTime, waveSpeed);
                }
            }

            attackTimer = currentWeapon.AttackCooldown / attackSpeedMultiplier;

        }
    }

    public void EndAttack()
    {
        Animator.SetBool("isHiting", false);
    }

    public void BulletAttack()
    {
        if (bulletProjectile != null)
        {
            GameObject prefab = bulletProjectile;

            float LifeTime = 10;
            float Speed = 15;

            Vector2 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;

            GameObject bullet = Instantiate(prefab, attackPoint.position, Quaternion.identity);

            WaveProjectile bp = bullet.GetComponent<WaveProjectile>();
            bp.Init(dir, 100, LifeTime, Speed);

            bulletCount--;
        }
    }

    public override void TakeDamage(float damage)
    {
        CurrentHealth -= damage - damage * damageResistance;

        StartCoroutine(DamageEffect());

        if (CurrentHealth <= 0 && !isDeath)
        {
            Die();
        }
    }

    public override void Move()
    {
        RigidBody.linearVelocity = moveInput*MoveSpeed;
    }

    private void EquipWeapon(int index)
    {
        if (index >= 0 || index < weapons.Length)
        {
            currentWeapon = weapons[index];
            currentWeapon.SetDisplay();
        }
    }

    private void FlipHorizontal()
    {
        facingDirection.x *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }
    private void FlipVerrtical()
    {
        facingDirection.y *= -1;
        Vector3 temp = attackPoint.transform.position;
        if (facingDirection.y > 0)
        {
            attackPoint.transform.position.Set(temp.y, temp.x, temp.z);
        }
        else
        {
            attackPoint.transform.position.Set(-temp.y, temp.x, temp.z);
        }
    }

    private void Rotate()
    {
        if (moveInput.x > 0 && facingDirection.x == -1 || moveInput.x < 0 && facingDirection.x == 1)
        {
            FlipHorizontal();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("ShieldPickup"))
        {
            if (activeShield == null)
            {
                activeShield = Instantiate(shieldEffectPrefab, transform.position, Quaternion.identity);
                activeShield.transform.parent = transform;
                Destroy(activeShield, 5f);
                if (isBoostActive)
                {
                    MoveSpeed = 5f;
                }
            }

            Destroy(collider.gameObject);
        }
        else if (collider.CompareTag("SpeedPickup"))
        {
            if (!isBoostActive)
            {
                StartCoroutine(SpeedBoostCoroutine(2.5f));
                if (activeShield != null)
                {
                    activeShield.SetActive(false);
                }
            }

            Destroy(collider.gameObject);
        }
        else if (collider.CompareTag("HeartPickup"))
        {
            if (CurrentHealth < MaxHealth - 5)
            {
                CurrentHealth += 5;
            }
            StartCoroutine(HealEffect());

            Destroy(collider.gameObject);
        }
        else if (collider.CompareTag("BulletPickup"))
        {
            if (bulletCount < 3)
            {
                bulletCount++;
                Destroy(collider.gameObject);
            }
        }
    }

    protected override void Die()
    {
        gameManager.GameOver();
        healthBar.SetActive(false);
        isDeath = true;
        Animator.SetBool("isDeath", isDeath);
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private IEnumerator Dash()
    {
        dashTimer = 0.5f;
        float elapsed = 0f;
        float duration = 0.3f;

        Vector3 from = transform.position;
        Vector3 to = transform.position + new Vector3(moveInput.x*3, moveInput.y*3, 0);
        while (elapsed < duration)
        {
            RigidBody.MovePosition(Vector3.Lerp(from, to, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator SpeedBoostCoroutine(float duration)
    {
        isBoostActive = true;

        float originalSpeed = MoveSpeed;
        MoveSpeed *= 2;

        yield return new WaitForSeconds(duration);

        MoveSpeed = originalSpeed;
        isBoostActive = false;
    }

    private IEnumerator HealEffect()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.color = Color.green;

        yield return new WaitForSeconds(0.15f);

        spriteRenderer.color = Color.white;
    }

    private IEnumerator DamageEffect()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.15f);

        spriteRenderer.color = Color.white;
    }

    public IEnumerator Knockback(Vector3 to)
    {
        float elapsed = 0f;
        float duration = 0.1f;

        Vector3 from = RigidBody.position;
        while (elapsed < duration)
        {
            RigidBody.MovePosition(Vector3.Lerp(from, to, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }
    }



    private void TimerUpdate()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        if (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(attackPosition, currentWeapon.AttackRange * attackRangeMultiplier);
    //}

}
