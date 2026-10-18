using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public float AttackCooldown {  get; protected set; }
    public float AttackDamage {  get; protected set; }
    public float Knockback {  get; protected set; }

    abstract public void Attack(GameObject attacker, Vector3 dir);

    abstract public void Init(GameObject wavePrefab);

}
