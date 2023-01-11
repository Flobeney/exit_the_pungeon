using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyBulletController : NetworkBehaviour
{
    // Champs
    public float Speed;

    // Lors d'une collision
    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Wall"){
            DestroyServerRpc();
        }
    }

    [ServerRpc]
    void DestroyServerRpc(){
        if(IsHost || IsServer){
            this.gameObject.GetComponent<NetworkObject>().Despawn();
            Destroy(this.gameObject);
        }
    }
}
