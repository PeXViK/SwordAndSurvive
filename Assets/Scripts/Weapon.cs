using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected float baseDamage;
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected string weaponName;
    [SerializeField] protected GameObject weaponDisplay;
    [SerializeField] protected Vector2 attackPosition;
    [SerializeField] public Sprite weaponIcon;
    [SerializeField] public GameObject weaponIconUI;
    private float waveLifeTime;

    public float BaseDamage { get => baseDamage; protected set => baseDamage = value; }
    public float AttackSpeed { get => attackSpeed; protected set => attackSpeed = value; }
    public float AttackRange { get => attackRange; protected set => attackRange = value; }
    public float AttackCooldown { get => attackCooldown; protected set => attackCooldown = value; }
    public float WaveLifeTime { get => waveLifeTime; protected set => waveLifeTime = value; }

    public GameObject WeaponIconUI { get => weaponIconUI; set => weaponIconUI = value; }
    public Sprite WeaponIcon { get => weaponIcon; set => weaponIcon = value; }

    virtual public void SetDisplay()
    {
        weaponDisplay.GetComponent<TextMeshProUGUI>().text = weaponName;
    }

    private void Start()
    {
        weaponDisplay = GameObject.FindGameObjectWithTag("WeaponName");
        weaponIconUI = GameObject.FindGameObjectWithTag("WeaponIcon");
    }

    public abstract void Attack(Player player, Vector3 attackPoint);

}
