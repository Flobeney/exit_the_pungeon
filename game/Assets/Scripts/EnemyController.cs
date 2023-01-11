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
    private GameObject[] _players;
    float IntervalMin = 5, IntervalMax = 7;
    float interval, startTime, endTime;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Init interval
        InitInterval();
    }

    // Update is called once per frame
    void Update()
    {
        if(IsHost || IsServer){
            // Fin attente interval
            if (Time.time >= endTime){
                // NewMovementServerRpc();
                UpdateServerRpc();
            }
        }
    }

    [ServerRpc]
    void UpdateServerRpc(){
        // Faire la différence entre la position du joueur et de l'ennemi
        // movement = (Player.transform.position - this.transform.position) / 10;
        movement = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);
        rb.velocity = movement * speed;
    }

    // Lors d'une collision
    void OnCollisionEnter2D(Collision2D other)
    {
        // Destroy enemy when colliding with bullet
        if(other.gameObject.tag == "Bullet"){
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

    // Init un nouvel interval
    private void InitInterval()
    {
        interval = Random.Range(IntervalMin, IntervalMax);
        startTime = Time.time;
        endTime = startTime + interval;
    }
}
