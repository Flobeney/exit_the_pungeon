using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletManager : MonoBehaviour
{
    // Champs
    public GameObject Player;
    public GameObject BulletPrefab;

    float IntervalMin = 0, IntervalMax = 3;
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
        if (Time.time >= endTime)
        {
            // Générer une balle
            GameObject bullet = Instantiate(BulletPrefab, this.transform.position, Quaternion.identity);
            // Donner une vitesse à la balle
            Vector3 movement = (Player.transform.position - this.transform.position) / 10;
            bullet.GetComponent<Rigidbody2D>().velocity = movement * bullet.GetComponent<EnemyBulletController>().Speed;

            // Nouvel interval
            InitInterval();
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
