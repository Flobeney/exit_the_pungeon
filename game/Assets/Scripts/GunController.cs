using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GunController : NetworkBehaviour
{
    [SerializeField] private GameObject bullet, shootPoint;
    [SerializeField] private float startTimeBtwShots;
    [SerializeField] private List<GameObject> spawnedBullets = new List<GameObject>();
    private float aimAngle, timeBtwShots;
    private Vector2 aim;

    // Start is called before the first frame update
    void Awake()
    {
        aim = new Vector2();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsOwner) return;

        // Rotation towards the mouse
        aim = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        aimAngle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(aimAngle, Vector3.forward);

        if (timeBtwShots <= 0)
        {
            if (Input.GetMouseButton(0))
            {
                ShootServerRpc();
                timeBtwShots = startTimeBtwShots;
            }
        }
        else
        {
            timeBtwShots -= Time.deltaTime;
        }
    }

    [ServerRpc]
    private void ShootServerRpc()
    {
        GameObject go = Instantiate(bullet, shootPoint.transform.position, transform.rotation);
        spawnedBullets.Add(go);
        go.GetComponent<BulletController>().parent = this;
        go.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyBulletServerRpc()
    {
        GameObject toDestroy = spawnedBullets[0];
        toDestroy.GetComponent<NetworkObject>().Despawn();
        spawnedBullets.Remove(toDestroy);
        Destroy(toDestroy);
    }

    // TODO: ServerRpc for particles and probably animations
}
