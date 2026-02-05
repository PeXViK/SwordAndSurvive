using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Rigidbody2D rigidBody;

    public float MaxHealth { get => maxHealth; protected set => maxHealth = value; }
    public float CurrentHealth { get => currentHealth; protected set => currentHealth = value; }
    public float MoveSpeed { get => moveSpeed; protected set => moveSpeed = value; }
    public Rigidbody2D RigidBody { get => rigidBody; protected set => rigidBody = value; }

    public abstract void Move();

    protected abstract void Die();

    public abstract void TakeDamage(float damage);
}

