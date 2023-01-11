using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyBulletManager : NetworkBehaviour
{
    // Champs
    public GameObject Player;
    public GameObject BulletPrefab;
    public GameObject[] Players;

    // Champs privés

    // float IntervalMin = 0, IntervalMax = 3;
    float IntervalMin = 5, IntervalMax = 7;
    float interval, startTime, endTime;

    // Start is called before the first frame update
    void Start()
    {
        // Init interval
        InitInterval();
    }

    // Update is called once per frame
    void Update()
    {
        // Fin attente interval
        if (Time.time >= endTime && (IsHost || IsServer))
        {
            UpdateServerRpc();
        }
    }

    [ServerRpc]
    void UpdateServerRpc(){
        // Générer une balle
        GameObject bullet = Instantiate(BulletPrefab, this.transform.position, Quaternion.identity);
        // Donner une vitesse à la balle
        Vector3 movement = (FindClosestPlayer() - this.transform.position) / 10;
        bullet.GetComponent<Rigidbody2D>().velocity = movement * bullet.GetComponent<EnemyBulletController>().Speed;
        bullet.GetComponent<NetworkObject>().Spawn();

        // Nouvel interval
        InitInterval();
    }

    // Trouve la position du joueur le plus proche
    private Vector3 FindClosestPlayer(){
        // Variables
        Vector3 posClosest = Vector3.zero;
        float distanceMin = 1000;
        float distance;

        // Parcourir les joueurs
        foreach (GameObject player in Players){
            // Calculer la distance entre le joueur et l'ennemi
            distance = Vector3.Distance(player.transform.position, this.transform.position);
            // Si la distance est plus petite que la distance minimale
            if(distance < distanceMin){
                distanceMin = distance;
                posClosest = player.transform.position;
            }
        }

        return posClosest;
    }

    // Init un nouvel interval
    private void InitInterval()
    {
        interval = Random.Range(IntervalMin, IntervalMax);
        startTime = Time.time;
        endTime = startTime + interval;
    }
}
