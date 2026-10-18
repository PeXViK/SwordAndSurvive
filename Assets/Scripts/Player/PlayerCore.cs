using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerCore : MonoBehaviour
{
    public static PlayerCore Instance { get; private set; }
    [SerializeField] private Transform attackPoint;
    [SerializeField] private GameObject shieldEffectPrefab;
    [SerializeField] private GameManager gameManager;

    public Rigidbody2D Rb {  get; private set; }
    public Animator Animator { get; private set; }
    public Transform AttackPoint => attackPoint;

    public bool isBoostActive = false;
    private PlayerInput input;
    private PlayerMovement movement;
    public GameUI uiManager { get; private set; }
    public PlayerHealth health;
    public PlayerProgression progression;
    private PlayerAnimation playerAnimation;
    public PlayerCombat combat;
    public GameObject activeShield;

    public bool IsDead { get; private set; }
    public Vector2 MoveInput => input != null ? input.MoveInput : Vector2.zero;

    private void Awake()
    {
        // Не допускаем появления второго игрока
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Сохраняем игрока при загрузке другой сцены
        DontDestroyOnLoad(gameObject);

        Rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        input = GetComponent<PlayerInput>();
        movement = GetComponent<PlayerMovement>();
        health = GetComponent<PlayerHealth>();
        progression = GetComponent<PlayerProgression>();
        combat = GetComponent<PlayerCombat>();
        playerAnimation = GetComponent<PlayerAnimation>();

        

        IsDead = false;
    }
    void Start()
    {
        movement.Init(this);
        health.Init(this);
        progression.Init(this);
        combat.Init(this);
    }

    public void Update()
    {
        uiManager.HealthBar.transform.localScale = new Vector3(health.CurrentHealth / health.MaxHealth, 1, 1);
        uiManager.ExpUI.text = "Опыт: " + progression.currentExp + "/" + progression.expToNextLevel;
        uiManager.LevelUI.text = "Уровень " + progression.currentLevel.ToString();

        if (gameManager == null)
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public void Die()
    {
        gameManager.GameOver();
        IsDead = true;
        uiManager.HealthBar.SetActive(false);
        Animator.SetBool("isDeath", IsDead);
        Rb.linearVelocity = Vector2.zero;
        Rb.constraints = RigidbodyConstraints2D.FreezeAll;
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
            }

            Destroy(collider.gameObject);
        }
        else if (collider.CompareTag("SpeedPickup"))
        {
            if (!isBoostActive)
            {
                movement.ApplySpeedBoost(2, 3);
            }

            Destroy(collider.gameObject);
        }
        else if (collider.CompareTag("HeartPickup"))
        {
            if (health.CurrentHealth < health.MaxHealth - 5)
            {
                health.Heal(5);
            }

            Destroy(collider.gameObject);
        }
    }

    public IEnumerator Knockback(Vector3 to)
    {
        float elapsed = 0f;
        float duration = 0.1f;

        Vector3 from = Rb.position;
        while (elapsed < duration)
        {
            Rb.MovePosition(Vector3.Lerp(from, to, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void ResetForNewGame()
    {
        health.ResetHealth();
        progression.ResetProgression();
        combat.ResetCombat();
        transform.position = Vector3.zero;

        IsDead = false;
        Animator.SetBool("isDeath", IsDead);
        Rb.linearVelocity = Vector2.zero;
        Rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void SetUIManager(GameUI manager)
    {
        uiManager = manager;
    }
}
