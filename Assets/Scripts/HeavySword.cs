using UnityEngine;
using UnityEngine.UI;

public class HeavySword : Weapon
{
    private void Awake()
    {
        BaseDamage = 25;
        AttackRange = 0.8f;
        AttackSpeed = 2;
        AttackCooldown = 1 / attackSpeed;
        WaveLifeTime = 0.2f;
        weaponName = "ׂÿזוכûי לוק";
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
