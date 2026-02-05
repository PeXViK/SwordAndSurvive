using UnityEngine;

public class PlayerProgression : MonoBehaviour
{
    [Header("Level & Experience")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float currentExp = 0f;
    [SerializeField] private float expToNextLevel = 10f;

    [Header("Progression Curve")]
    [SerializeField] private float expIncreasePerLevel = 1.2f;
    [SerializeField] private float healthPerLevel = 10f; 
    [SerializeField] private float healingPerLevel = 20f;
    [SerializeField] private float attackSpeedPerLevel = 0.02f;
    [SerializeField] private float attackRangePerLevel = 0.01f;
    [SerializeField] private float attackDamagePerLevel = 0.05f;
    [SerializeField] private float damageResistansePerLevel = 0.05f;

    private PlayerCore core;
    private PlayerHealth health;
    private PlayerCombat combat;

    public void Init(PlayerCore coreRef)
    {
        core = coreRef;
    }

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
        combat = GetComponent<PlayerCombat>();
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
        expToNextLevel *= expIncreasePerLevel;

        health.IncreaseMaxHealth(healthPerLevel);
        health.Heal(healingPerLevel);
        // Сюды анимацию и звук
        Debug.Log($"Level Up! Теперь уровень {currentLevel}");
    }
}
