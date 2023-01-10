using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyBulletController : MonoBehaviour
{
    // Champs
    public float Speed;

    // Lors d'une collision
    void OnCollisionEnter2D(Collision2D other)
    {
        DestroyServerRpc(other);
    }

    [ServerRpc]
    void DestroyServerRpc(Collision2D other){
        if(other.gameObject.tag == "Wall"){
            this.gameObject.GetComponent<NetworkObject>().Despawn();
            Destroy(this.gameObject);
        }
    }
}
