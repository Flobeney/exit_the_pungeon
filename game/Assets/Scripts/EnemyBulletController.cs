using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletController : MonoBehaviour
{
    // Champs
    public float Speed;

    // Lors d'une collision
    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Wall"){
            Destroy(this.gameObject);
        }
    }
}
