using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private float arenaBorder;
    private Transform player;
    private Vector3 position;
    private Vector3 cameraPosition;
    
    void Start()
    {   
        cameraPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            position = player.position;
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        if (Vector2.Distance(position, cameraPosition) > 1)
        { 
            cameraPosition.x += (position.x - cameraPosition.x) * 0.1f;
            cameraPosition.y += (position.y - cameraPosition.y) * 0.1f;
        }
        transform.position = cameraPosition; 
    }

    
}
