using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    private PlayerCore core;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    public void Init(PlayerCore coreRef)
    {
        Debug.Log("HealthSystem Initializing");
        core = coreRef;
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (core.IsDead) return;

        currentHealth -= (damage - damage * (core.progression.DamageResistanseModifier - 1)); // присрать сопротивление урону - готово
        StartCoroutine(DamageFlash());

        if (core.uiManager.HealthBar != null)
            core.uiManager.HealthBar.transform.localScale = new Vector3(currentHealth / maxHealth, 1f, 1f);

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
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
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

    public void ResetHealth()
    {
        maxHealth = 100;
        currentHealth = maxHealth;

        if (core.uiManager.HealthBar != null)
        {
            core.uiManager.HealthBar.transform.localScale =
                new Vector3(1f, 1f, 1f);
        }
    }
}