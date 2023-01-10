using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyController : NetworkBehaviour
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
        if(IsHost || IsServer){
            UpdateServerRpc();
        }
    }

    [ServerRpc]
    void UpdateServerRpc(){
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
            DestroyServerRpc();
        }
    }

    [ServerRpc]
    void DestroyServerRpc(){
        if(IsHost || IsServer){
            this.gameObject.GetComponent<NetworkObject>().Despawn();
            Destroy(this.gameObject);
            GameObject.Find("EnemyGenerator").GetComponent<EnemyGenerator>().EnemyDestroyed();
        }
    }
}
