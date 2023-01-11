using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorControls : MonoBehaviour
{
    private Vector2 mousePos;

    // Start is called before the first frame update
    void Awake()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Camera currentCam = Camera.current != null ? Camera.current : Camera.main;
        mousePos = currentCam.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePos;
    }
}
