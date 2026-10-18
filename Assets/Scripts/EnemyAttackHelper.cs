using UnityEngine;

public class EnemyAttackHelper : MonoBehaviour
{
    private Enemy enemy;
    private Boss boss;
    private bool isInCollider = false;
    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        boss = GetComponent<Boss>();
    }


    private void Attack() {
        Debug.Log("Вызов из хелпера");
        if (isInCollider && enemy != null)
        {
            enemy.DoingAttack();
        }
        if (isInCollider && boss != null)
        {
            boss.DoingAttack();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Игрок вошёл в зону!");
            isInCollider = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Игрок вышел из зоны!");
            isInCollider = false;
        }
    }
}
