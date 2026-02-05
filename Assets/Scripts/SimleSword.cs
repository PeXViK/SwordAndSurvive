using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleSword : Weapon
{
    
    private void Awake()
    {
        BaseDamage = 15;
        AttackRange = 0.6f;
        AttackSpeed = 2.5f;
        AttackCooldown = 1 / attackSpeed;
        WaveLifeTime = 0.2f;
        weaponName = "Простой меч";
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
