using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float baseMoveSpeed = 5f;
    private PlayerCore core;
    private PlayerInput input;
    private float currentSpeedMultiplier = 1f;
    private float dashTimer;

    public void Init(PlayerCore coreRef)
    {
        core = coreRef;
        input = core.GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (dashTimer > 0) dashTimer -= Time.deltaTime;
    }
    void FixedUpdate()
    {
        if (core.IsDead) return;

        core.Rb.linearVelocity = input.MoveInput * baseMoveSpeed * currentSpeedMultiplier;
    }

    // Вызывать из PlayerCore или из Update какого-то компонента
    public void TryDash()
    {
        if (dashTimer > 0 || input.MoveInput == Vector2.zero) return;
        StartCoroutine(PerformDash());
    }

    private IEnumerator PerformDash()
    {
        dashTimer = 0.5f;
        float duration = 0.3f;
        float elapsed = 0f;
        Vector3 from = transform.position;
        Vector3 to = from + (Vector3)(input.MoveInput * 3f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            core.Rb.MovePosition(Vector3.Lerp(from, to, elapsed / duration));
            yield return null;
        }
    }

    // Для баффа скорости (pickup)
    public void ApplySpeedBoost(float multiplier, float duration)
    {
        currentSpeedMultiplier = multiplier;
        StartCoroutine(ResetSpeedAfter(duration));
    }

    private IEnumerator ResetSpeedAfter(float dur)
    {
        yield return new WaitForSeconds(dur);
        currentSpeedMultiplier = 1f;
    }
}