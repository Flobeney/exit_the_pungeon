using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CursorControls : NetworkBehaviour
{
    private Vector2 mousePos;

    // Start is called before the first frame update
    void Awake()
    {
        //Cursor.visible = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
<<<<<<< Updated upstream
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
=======
        if (!IsServer) return;
        Camera currentCam = Camera.current != null ? Camera.current : Camera.main;
        mousePos = currentCam.ScreenToWorldPoint(Input.mousePosition);
>>>>>>> Stashed changes
        transform.position = mousePos;
    }
}
