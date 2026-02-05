using UnityEngine;
using UnityEngine.UI;

public class LongSword : Weapon
{
    private void Awake()
    {
        BaseDamage = 10;
        AttackRange = 1.0f;
        AttackSpeed = 1.5f;
        AttackCooldown = 1 / attackSpeed;
        WaveLifeTime = 0.3f;
        weaponName = "Длинный меч";
    }
    public override void Attack(Player player, Vector3 attackPoint)
    {
        float damage = BaseDamage * player.AttackDamageMultiplier;
        float range = AttackRange * player.AttackRangeMultiplier;

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint, range);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.GetComponent<Enemy>().TakeDamage(damage);
            }
        }

    }
    public override void SetDisplay()
    {
        base.SetDisplay();
        
        
    }

}
