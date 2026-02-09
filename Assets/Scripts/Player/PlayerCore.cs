using UnityEngine;

public class PlayerCore : MonoBehaviour
{

    public Rigidbody2D Rb {  get; private set; }
    public Animator Animator { get; private set; }
    public Transform AttackPoint { get; private set; }

    private PlayerInput input;
    private PlayerMovement movement;
    private PlayerHealth health;
    private PlayerProgression progression;
    private PlayerAnimation playerAnimation;
    public PlayerCombat combat;

    public bool IsDead { get; private set; }
    public Vector2 MoveInput => input != null ? input.MoveInput : Vector2.zero;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        
        input = GetComponent<PlayerInput>();
        movement = GetComponent<PlayerMovement>();
        health = GetComponent<PlayerHealth>();
        progression = GetComponent<PlayerProgression>();
        combat = GetComponent<PlayerCombat>();
        playerAnimation = GetComponent<PlayerAnimation>();

        AttackPoint = GetComponentInChildren<Transform>();

        IsDead = false;
    }
    void Start()
    {
        movement.Init(this);
        health.Init(this);
        progression.Init(this);
        combat.Init(this);
    }

    public void Die()
    {
        IsDead = true;
    }
}
