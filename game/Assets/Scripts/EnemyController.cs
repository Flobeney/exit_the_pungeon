using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Champs
    public GameObject Player;

    private Rigidbody2D rb;
    private Vector3 movement;
    private float speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Faire la différence entre la position du joueur et de l'ennemi
        movement = (Player.transform.position - this.transform.position) / 10;
        rb.velocity = movement * speed;
    }

    // Lors d'une collision
    void OnCollisionEnter2D(Collision2D other)
    {
        // WORKAROUND : destroy enemy when colliding with player
        // TODO : destroy enemy when colliding with bullet
        if(other.gameObject.tag == "Player"){
            Destroy(this.gameObject);
            GameObject.Find("EnemyGenerator").GetComponent<EnemyGenerator>().EnemyDestroyed();
        }
    }
}
