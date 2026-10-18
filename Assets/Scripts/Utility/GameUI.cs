using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    private void Start()
    {
        if (PlayerCore.Instance != null)
        {
            PlayerCore.Instance.SetUIManager(this);
        }
    }

    [SerializeField] public TextMeshProUGUI WeaponUI;
    [SerializeField] public TextMeshProUGUI HealthUI;
    [SerializeField] public TextMeshProUGUI ExpUI;
    [SerializeField] public TextMeshProUGUI LevelUI;
    [SerializeField] public TextMeshProUGUI GameTimer;
    [SerializeField] public GameObject HealthBar;

}
