using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    // Player variables
    [SerializeField]
    private float speed = 5f;
    public GameObject player;
    private Rigidbody2D rbPlayer;
    private float horizontalForce, verticalForce;
    private Vector2 movement, cursor;
    private bool facingRight = true;

    // Start is called before the first frame update
    private void Awake()
    {
        rbPlayer = GetComponent<Rigidbody2D>();
        movement = new Vector2();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        if (cursor.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (cursor.x < 0 && facingRight)
        {
            Flip();
        }

        // Movements with Rigidbody2D
        horizontalForce = Input.GetAxis("Horizontal");
        verticalForce = Input.GetAxis("Vertical");
        movement.x = horizontalForce;
        movement.y = verticalForce;
        rbPlayer.velocity = movement * speed;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = player.transform.localScale;
        theScale.x *= -1;
        player.transform.localScale = theScale;
    }
}
