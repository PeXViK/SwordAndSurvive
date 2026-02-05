using UnityEngine;
using System.Collections.Generic;

public class WaveProjectile : MonoBehaviour
{
    public float speed;
    public float lifeTime;
    public float damage;
    private Animator anim;

    private Vector2 direction;
    private HashSet<Entity> damaged = new HashSet<Entity>();

    public void Init(Vector2 dir, float damage, float lifeTime, float speed)
    {
        direction = dir.normalized;
        this.damage = damage;
        this.lifeTime = lifeTime;
        this.speed = speed;

        anim = GetComponent<Animator>();
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        float animLength = state.length;
        anim.speed = animLength / lifeTime;

        // Поворачиваем волну в направлении полёта
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Start()
    {
        if (lifeTime != 0) { 
            Destroy(gameObject, lifeTime);
        }
        else
        {
            Destroy(gameObject, 1f);
        }
    }

    private void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Entity e = other.GetComponent<Entity>();
            if (e != null && !damaged.Contains(e))
            {
                damaged.Add(e);
                e.TakeDamage(damage);
            }
        }
    }
}