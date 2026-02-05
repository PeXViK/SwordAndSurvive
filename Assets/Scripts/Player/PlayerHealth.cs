using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private PlayerCore core;
    private GameObject healthBar;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    public void Init(PlayerCore coreRef)
    {
        core = coreRef;
        currentHealth = maxHealth;
        healthBar = transform.Find("HealthBar")?.gameObject;
    }

    public void TakeDamage(float damage)
    {
        if (core.IsDead) return;

        currentHealth -= damage; // присрать сопротивление урону
        StartCoroutine(DamageFlash());

        if (healthBar != null)
            healthBar.transform.localScale = new Vector3(currentHealth / maxHealth, 1f, 1f);

        if (currentHealth <= 0)
            core.Die();
    }

    public void Heal(float value)
    {
        if (value + currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
            return;
        }
        currentHealth += value;
        StartCoroutine(HealFlash());
    }

    public void IncreaseMaxHealth(float value)
    {
        maxHealth += value;
    }

    private IEnumerator DamageFlash()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr == null) yield break;
        sr.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        sr.color = Color.white;
    }

    private IEnumerator HealFlash()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = Color.green;
        yield return new WaitForSeconds(0.15f);
        sr.color = Color.white;
    }
}