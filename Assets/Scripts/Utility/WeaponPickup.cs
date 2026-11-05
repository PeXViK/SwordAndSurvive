using Assets.Scripts.Utility;
using System;
using TMPro;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private ItemData itemData;

    private bool playerInRange;
    private PlayerCombat playerCombat;

    private void Update()
    {
        if (!playerInRange)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (playerCombat == null || itemData == null)
                return;

            Debug.Log("Input.GetKeyDown(KeyCode.E)");

            playerCombat.EquipWeapon(itemData);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("OnTriggerEnter2D");
            PlayerCombat combat = other.GetComponentInParent<PlayerCombat>();
            if (combat != null)
            {
                playerCombat = combat;
                playerInRange = true;
            }
            PickupMsgUI.Instance?.Show();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            Debug.Log("OnTriggerExit2D");
            PlayerCombat combat = other.GetComponentInParent<PlayerCombat>();
            if (combat != null && combat == playerCombat)
            {
                playerInRange = false;
                playerCombat = null;
            }
                PickupMsgUI.Instance?.Hide();
        }
    }
}
