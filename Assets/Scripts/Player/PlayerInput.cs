using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 AimDirection { get; private set; }
    public bool IsAttackPressed { get; private set; }
    public bool IsSpecialAttackPressed { get; private set; }
    public bool IsDashPressed { get; private set; }
    public int WeaponSlotInput { get; private set; } = -1;

    public Vector3 MousePosition { get; private set; }

    void Update()
    {
        MoveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        AimDirection = (mouse - transform.position).normalized;

        IsAttackPressed = Input.GetMouseButton(0);
        IsSpecialAttackPressed = Input.GetMouseButtonDown(1);
        IsDashPressed = Input.GetButtonDown("Jump");
        MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.Alpha1)) WeaponSlotInput = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) WeaponSlotInput = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) WeaponSlotInput = 2;
    }
}