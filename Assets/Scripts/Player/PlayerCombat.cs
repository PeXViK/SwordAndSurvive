using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Weapon currentWeapon;
    [SerializeField] private Weapon[] weapons;
    [SerializeField] private GameObject simpleWave;
    [SerializeField] private GameObject longWave;
    [SerializeField] private GameObject heavyWave;
    [SerializeField] private GameObject bulletProjectile;

    private int currentSlot = 0;
    private int bulletCount = 0;
    private float attackTimer = 0;
    private int weaponCount;

    private PlayerProgression stats;
    private PlayerInput input;
    private PlayerCore core;

    public void Init(PlayerCore coreRef)
    {
        Debug.Log("CombatSystem Initializing");
        core = coreRef;
        stats = core.GetComponent<PlayerProgression>();
        input = core.GetComponent<PlayerInput>();
        weapons[0] = gameObject.AddComponent<SimpleSword>();
        weapons[1] = gameObject.AddComponent<LongSword>();
        weapons[2] = gameObject.AddComponent<HeavySword>();
        currentWeapon = weapons[0];
        weaponCount = weapons.Length;
    }


    void Update()
    {
        if (attackTimer > 0) attackTimer -= Time.deltaTime;
        if (input.WeaponSlotInput >= 0 && input.WeaponSlotInput != currentSlot)
        {
            currentSlot = input.WeaponSlotInput;
            currentWeapon = weapons[currentSlot];
            Debug.Log($"Weapon slot: {input.WeaponSlotInput}");
            Debug.Log($"Weapons Array Length: {weapons.Length}");
        }

        if (input.IsAttackPressed) SwordAttack();
    }

    public void SwordAttack()
    {
        if (currentWeapon != null && attackTimer <= 0)
        {
            attackTimer = currentWeapon.AttackCooldown / stats.AttackSpeedModifier;

            GameObject prefab = null;
            if (currentSlot == 0) prefab = simpleWave;
            if (currentSlot == 1) prefab = longWave;
            if (currentSlot == 2) prefab = heavyWave;

            float waveLifeTime = currentWeapon.WaveLifeTime;
            float waveSpeed = currentWeapon.AttackRange / waveLifeTime;

            if (prefab != null)
            {
                Vector2 dir = (input.MousePosition - transform.position).normalized;

                GameObject wave = Instantiate(prefab, core.AttackPoint.position, Quaternion.identity);
                Debug.Log(dir.ToString());

                WaveProjectile wp = wave.GetComponent<WaveProjectile>();
                wp.Init(dir, currentWeapon.BaseDamage * stats.AttackDamageModifier, waveLifeTime, waveSpeed);
            }
        }
    }
    public void BulletAttack()
    {
        if (bulletProjectile != null)
        {
            GameObject prefab = bulletProjectile;

            float LifeTime = 10;
            float Speed = 15;

            Vector2 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;

            GameObject bullet = Instantiate(prefab, core.AttackPoint.position, Quaternion.identity);

            WaveProjectile bp = bullet.GetComponent<WaveProjectile>();
            bp.Init(dir, 100, LifeTime, Speed);

            bulletCount--;
        }
    }
}
