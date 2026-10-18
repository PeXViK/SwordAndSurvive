using TMPro;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private ItemData startingWeapon;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private GameObject bulletProjectile;

    private ItemData currentWeapon;
    private float attackTimer;

    private PlayerCore core;
    private PlayerInput input;
    private PlayerProgression stats;
    private PlayerAnimation playerAnimation;
    public int bulletCount = 0;

    public ItemData CurrentWeapon => currentWeapon;

    public void Init(PlayerCore coreRef)
    {
        core = coreRef;
        input = core.GetComponent<PlayerInput>();
        stats = core.GetComponent<PlayerProgression>();
        playerAnimation = core.GetComponent<PlayerAnimation>();

        if (attackPoint == null)
            attackPoint = core.AttackPoint;

        if (startingWeapon != null)
            EquipWeapon(startingWeapon, false);
    }

    private void Update()
    {
        core.uiManager.WeaponUI.text = currentWeapon.itemName;

        if (input == null)
            return;

        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        if (input.IsAttackPressed)
            SwordAttack();

        if (input.IsSpecialAttackPressed)
            BulletAttack();
    }

    public void EquipWeapon(ItemData newWeapon, bool dropCurrent = true)
    {
        if (newWeapon == null)
            return;

        ItemData oldWeapon = currentWeapon;

        // Экипируем новый меч
        currentWeapon = newWeapon;

        if (playerAnimation != null)
        {
            playerAnimation.SetWeaponAnimation(newWeapon);
        }

        // Спавним старый на землю
        if (dropCurrent && oldWeapon != null && oldWeapon != newWeapon)
        {
            if (oldWeapon.prefab != null)
            {
                Instantiate(
                    oldWeapon.prefab,
                    core.transform.position,
                    Quaternion.identity
                );
            }
        }

        Debug.Log("Экипирован меч: " + currentWeapon.itemName);
    }

    public void SwordAttack()
    {
        Debug.Log("Попытка нанести атаку");
        if (currentWeapon == null || attackTimer > 0)
            return;

        if (currentWeapon.projectilePrefab == null)
            return;

        if (playerAnimation != null)
        {
            playerAnimation.PlayAttackAnimation();
        }

        attackTimer = currentWeapon.attackCooldown /
                      Mathf.Max(0.01f, stats.AttackSpeedModifier);

        Vector3 direction = input.AimDirection;

        GameObject wave = Instantiate(
            currentWeapon.projectilePrefab,
            attackPoint.position,
            Quaternion.identity
        );

        WaveProjectile projectile = wave.GetComponent<WaveProjectile>();

        if (projectile != null)
        {
            projectile.Init(
                direction,
                currentWeapon.baseDamage * stats.AttackDamageModifier,
                currentWeapon.projectileLifetime,
                currentWeapon.projectileSpeed
            );
        }
    }

    public void BulletAttack()
    {
        if (bulletProjectile != null && bulletCount > 0 && !core.IsDead)
        {
            bulletCount--;

            GameObject prefab = bulletProjectile;

            float LifeTime = 10;
            float Speed = 15;

            Vector2 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;

            GameObject bullet = Instantiate(prefab, attackPoint.position, Quaternion.identity);

            WaveProjectile bp = bullet.GetComponent<WaveProjectile>();
            bp.Init(dir, 100, LifeTime, Speed);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("BulletPickup"))
        {
            if (bulletCount < 3)
            {
                bulletCount++;
                Destroy(collider.gameObject);
            }
        }
    }

    public void ResetCombat() {
        if (startingWeapon != null)
            EquipWeapon(startingWeapon, false);
    }
}
