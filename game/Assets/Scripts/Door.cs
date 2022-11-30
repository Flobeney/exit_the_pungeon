using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Champs
    private bool _firstEnter = true;
    private bool _exit = true;
    private Vector3 _currentCameraPosition;
    private Vector3 _newCameraPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Called when the player enters the door
    /// </summary>
    void OnTriggerEnter2D(Collider2D other){
        // Si le joueur entre dans la porte
        if(other.name == "Player"){
            // Si c'est la première fois qu'il entre dans la porte
            if(_firstEnter){
                // On génère la nouvelle pièce
                GenerateNextRoom();
                _firstEnter = false;
            }else{
                // Le joueur est déjà passé par la porte, il faut juste changer la position de la caméra
                Camera.main.transform.position = _exit ? _currentCameraPosition : _newCameraPosition;
                _exit = !_exit;
            }
        }
    }

    /// <summary>
    /// Generate the next room
    /// </summary>
    void GenerateNextRoom(){
        // Get size of camera
        Vector3 min = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 max = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));

        // Save current camera position
        _currentCameraPosition = Camera.main.transform.position;

        float diff;
        DoorDirection doorDirection = DoorDirection.None;
        switch (DoorDirection.Parse(typeof(DoorDirection), this.tag)){
            // Horizontal
            case DoorDirection.DoorTop:
                diff = max.y - min.y;

                min.y = max.y;
                max.y += diff;

                doorDirection = DoorDirection.DoorBottom;
                break;
            case DoorDirection.DoorBottom:
                diff = max.y - min.y;

                max.y = min.y;
                min.y -= diff;

                doorDirection = DoorDirection.DoorTop;
                break;
            // Vertical
            case DoorDirection.DoorRight:
                diff = max.x - min.x;

                min.x = max.x;
                max.x += diff;

                doorDirection = DoorDirection.DoorLeft;
                break;
            case DoorDirection.DoorLeft:
                diff = max.x - min.x;

                max.x = min.x;
                min.x -= diff;

                doorDirection = DoorDirection.DoorRight;
                break;
            default:
                break;
        }

        // Sauver la nouvelle position de la caméra
        _newCameraPosition = new Vector3((max.x + min.x) / 2, (max.y + min.y) / 2, Camera.main.transform.position.z);

        // Ne pas générer de là où on vient
        FindObjectOfType<LevelGenerator>().GenerateRoom(min, max, doorDirection);
    }
}
