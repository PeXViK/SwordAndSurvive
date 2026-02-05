using UnityEngine;
using System.Collections.Generic;

public class BreadBulletProjectile : MonoBehaviour
{
    public float speed;
    public float lifeTime;
    public float damage;

    private Vector2 direction;
    private HashSet<Player> damaged = new HashSet<Player>();

    public void Init(Vector2 dir, float damage, float lifeTime, float speed)
    {
        direction = dir.normalized;
        this.damage = damage;
        this.lifeTime = lifeTime;
        this.speed = speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player e = other.GetComponent<Player>();
            if (e != null && !damaged.Contains(e))
            {
                damaged.Add(e);
                e.TakeDamage(damage);
                e.StartCoroutine(e.Knockback(e.transform.position + new Vector3(direction.x, direction.y, 0)));
                Destroy(gameObject);
            }
        }
    }


}