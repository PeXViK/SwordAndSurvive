using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Items/Weapon")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public float baseDamage = 15f;
    public float attackCooldown = 1f;
    public Sprite icon;

    [Header("World")]
    public GameObject prefab;

    [Header("Attack")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 1f;
    public float projectileLifetime = 1f;

    [Header("Animation")]
    public string attackAnimationTrigger = "Attack";
}