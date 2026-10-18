using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LongSword : BaseSword
{
    private void Start()
    {
        WaveSpeed = 1;
        WaveLifetime = 1;
        Name = "Simple Sword";
    }

    public override void Attack(GameObject attacker, Vector3 dir)
    {
        PlayerCore player = attacker.GetComponent<PlayerCore>();

        GameObject wave = Instantiate(WaveProj, player.AttackPoint.position, Quaternion.identity);
        Debug.Log(dir.ToString());

        WaveProjectile wp = wave.GetComponent<WaveProjectile>();
        wp.Init(dir, AttackDamage * player.progression.AttackDamageModifier, WaveLifetime, WaveSpeed);
    }
    public override void Init(GameObject wavePrefab)
    {
        WaveProj = wavePrefab;
    }
}
