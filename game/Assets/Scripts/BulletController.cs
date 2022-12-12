using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletController : NetworkBehaviour
{
    [SerializeField] private float speed, lifeTime;

    public GunController parent;

    void Start()
    {
        Invoke("DestroyBullet", lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void DestroyBullet()
    {
        if (!IsOwner) return;
        parent.DestroyBulletServerRpc();
    }
}
