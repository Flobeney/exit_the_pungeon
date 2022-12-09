using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    private float aimAngle, timeBtwShots;
    private Vector2 aim;

    public GameObject bullet, shootPoint;
    public float startTimeBtwShots;

    // Start is called before the first frame update
    void Awake()
    {
        aim = new Vector2();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Rotation towards the mouse
        aim = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        aimAngle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(aimAngle, Vector3.forward);

        if (timeBtwShots <= 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Instantiate(bullet, shootPoint.transform.position, transform.rotation);
                timeBtwShots = startTimeBtwShots;
            }
        }
        else
        {
            timeBtwShots -= Time.deltaTime;
        }
    }
}
