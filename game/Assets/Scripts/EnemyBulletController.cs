using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletController : MonoBehaviour
{
    // Champs
    public float Speed;

    // Lors d'une fin de collision (trigger)
    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag.Contains("Door")){
            Destroy(this.gameObject);
        }
    }
}
