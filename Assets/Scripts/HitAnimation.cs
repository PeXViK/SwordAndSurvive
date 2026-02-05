using Unity.VisualScripting;
using UnityEngine;

public class HitAnimation : MonoBehaviour {
    public Player player;

    private void Update()
    {
        if (player.MoveInput.y > 0)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        if (player.MoveInput.y < 0)
        {
            transform.localRotation = Quaternion.Euler(0, 0, -90);
        }
        if (player.MoveInput.y == 0)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        if (player.MoveInput.y > 0 && player.MoveInput.x != 0)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 45);
        }
        if (player.MoveInput.y < 0 && player.MoveInput.x != 0)
        {
            transform.localRotation = Quaternion.Euler(0, 0, -45);
        }
    }

    

    
}
