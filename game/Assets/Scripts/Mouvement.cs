using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouvement : MonoBehaviour
{
    private Rigidbody2D rb;
    private float speed = 5f, horizontalForce, verticalForce;
    private Vector2 movement;
    // Start is called before the first frame update
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        // Movements with Rigidbody2D
        horizontalForce = Input.GetAxis("Horizontal");
        verticalForce = Input.GetAxis("Vertical");
        movement = new Vector2(horizontalForce, verticalForce);
        rb.velocity = movement * speed;
    }
}
