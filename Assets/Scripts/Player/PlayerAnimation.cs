using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private ParticleSystem walkParticles;
    [SerializeField] private ParticleSystem dashParticles;
    private Animator animator;
    private PlayerInput input;
    private ItemData currentWeapon;
    private Vector3 facingDirection = new Vector3(1, 1, 0);

    private void Awake()
    {
        animator = GetComponent<Animator>();
        input = GetComponent<PlayerInput>();
    }

    private void FixedUpdate()
    {
        animator.SetFloat("horizontal", Mathf.Abs(input.MoveInput.x));
        animator.SetFloat("vertical", Mathf.Abs(input.MoveInput.y));

        Rotate();

        if (input.MoveInput.normalized != Vector2.zero)
        {
            Debug.Log("Партиклы " + walkParticles.isPlaying);
            if (!walkParticles.isPlaying)
                walkParticles.Play();
        }
        else
        {
            if (walkParticles.isPlaying)
                walkParticles.Stop();
        }
    }

    public void SetWeaponAnimation(ItemData weapon)
    {
        currentWeapon = weapon;
    }

    public void PlayAttackAnimation()
    {
        if (animator == null || currentWeapon == null)
            return;

        string trigger = currentWeapon.attackAnimationTrigger;

        if (!string.IsNullOrEmpty(trigger))
        {
            animator.SetTrigger(trigger);
        }
    }

    private void Rotate()
    {
        void FlipHorizontal()
        {
            facingDirection *= -1;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }

        if (input.MoveInput.x > 0 && facingDirection.x == -1 || input.MoveInput.x < 0 && facingDirection.x == 1)
        {
            FlipHorizontal();
        }
    }

    public void DashAnim()
    {
        Vector3 dashStartPosition = transform.position;

        if (dashParticles != null)
        {
            Instantiate(
                dashParticles,
                dashStartPosition,
                Quaternion.identity
            ).Play();
        }
    }
}
