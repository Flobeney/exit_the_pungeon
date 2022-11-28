using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouvement : MonoBehaviour
{
    // Other player variables
    public GameObject otherPlayer;
    private Rigidbody2D otherPlayerRb;

    // Player variables
    private Rigidbody2D rb;
    private float speed = 5f, horizontalForce, verticalForce;
    private Vector2 movement;

    
    // Start is called before the first frame update
    private void Awake()
    {
        otherPlayerRb = otherPlayer.GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        movement = new Vector2();
    }

    private void FixedUpdate()
    {
        // Movements with Rigidbody2D
        horizontalForce = Input.GetAxis("Horizontal");
        verticalForce = Input.GetAxis("Vertical");
        movement.x = horizontalForce;
        movement.y = verticalForce;
        rb.velocity = movement * speed;
    }
}
