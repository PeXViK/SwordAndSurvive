using UnityEngine;

public class PlayerProgression : MonoBehaviour
{
    [Header("Level & Experience")]
    [SerializeField] public int currentLevel { get; private set; } = 1;
    [SerializeField] public float currentExp { get; private set; } = 0f;
    [SerializeField] public float expToNextLevel { get; private set; } = 10;

    [Header("Progression Curve")]
    [SerializeField] private float expIncreasePerLevel = 2;
    [SerializeField] private float healthPerLevel = 10f; 
    [SerializeField] private float healingPerLevel = 20f;
    [SerializeField] private float attackSpeedPerLevel = 0.02f;
    [SerializeField] private float attackRangePerLevel = 0.01f;
    [SerializeField] private float attackDamagePerLevel = 0.05f;
    [SerializeField] private float damageResistansePerLevel = 0.05f;

    public float AttackSpeedModifier { get; private set; } = 1f;
    public float AttackRangeModifier { get; private set; } = 1f;
    public float AttackDamageModifier { get; private set; } = 1f;
    public float DamageResistanseModifier { get; private set; } = 1f;

    private PlayerCore core;
    private PlayerHealth health;
    private PlayerCombat combat;

    public void Init(PlayerCore coreRef)
    {
        Debug.Log("LevelSystem Initializing");
        core = coreRef;
        health = core.GetComponent<PlayerHealth>();
        combat = core.GetComponent<PlayerCombat>();
    }

    private void Update()
    {
        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    public void IncreaseExperience(float value)
    {
        currentExp += value;
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExp -= expToNextLevel;
        expToNextLevel += expIncreasePerLevel;
        AttackDamageModifier += attackDamagePerLevel;
        AttackSpeedModifier += attackSpeedPerLevel;
        AttackRangeModifier += attackRangePerLevel;
        DamageResistanseModifier += damageResistansePerLevel;

        health.IncreaseMaxHealth(healthPerLevel);
        health.Heal(healingPerLevel);
        // Сюды анимацию и звук
        Debug.Log($"Level Up! Теперь уровень {currentLevel}");
    }

    public void ResetProgression()
    {
        currentLevel= 1;
        currentExp = 0f;
        expToNextLevel = 10;

        expIncreasePerLevel = 2;
        healthPerLevel = 10f;
        healingPerLevel = 20f;
        attackSpeedPerLevel = 0.02f;
        attackRangePerLevel = 0.01f;
        attackDamagePerLevel = 0.05f;
        damageResistansePerLevel = 0.05f;
    }
}
