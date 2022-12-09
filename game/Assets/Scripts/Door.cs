using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Champs
    private DoorDirection _direction;

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
        if(other.tag == "Player"){
            // Get DoorDirection
            _direction = (DoorDirection)DoorDirection.Parse(typeof(DoorDirection), this.tag);

            // On génère la nouvelle pièce
            GenerateNextRoom(GetNextDoorDirection(other));
        }
    }

    /// <summary>
    /// Compute the next door direction
    /// </summary>
    /// <param name="player">The collider of the player</param>
    /// <returns>The direction of the next door</returns>
    DoorDirection GetNextDoorDirection(Collider2D player){
        DoorDirection directionNextRoom = DoorDirection.None;
        switch (_direction){
            // Horizontal
            case DoorDirection.DoorTop:
                directionNextRoom = player.transform.position.y > transform.position.y ? DoorDirection.DoorBottom : DoorDirection.DoorTop;
                break;
            case DoorDirection.DoorBottom:
                directionNextRoom = player.transform.position.y < transform.position.y ? DoorDirection.DoorTop : DoorDirection.DoorBottom;
                break;
            // Vertical
            case DoorDirection.DoorRight:
                directionNextRoom = player.transform.position.x > transform.position.x ? DoorDirection.DoorLeft : DoorDirection.DoorRight;
                break;
            case DoorDirection.DoorLeft:
                directionNextRoom = player.transform.position.x < transform.position.x ? DoorDirection.DoorRight : DoorDirection.DoorLeft;
                break;
            default:
                break;
        }

        return directionNextRoom;
    }

    /// <summary>
    /// Generate the next room
    /// </summary>
    void GenerateNextRoom(DoorDirection directionNextRoom){
        // Get size of camera
        Vector3 min = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 max = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));

        float diff;
        switch (directionNextRoom){
            // Horizontal
            case DoorDirection.DoorTop:
                diff = max.y - min.y;

                min.y = max.y;
                max.y += diff;
                break;
            case DoorDirection.DoorBottom:
                diff = max.y - min.y;

                max.y = min.y;
                min.y -= diff;
                break;
            // Vertical
            case DoorDirection.DoorRight:
                diff = max.x - min.x;

                min.x = max.x;
                max.x += diff;
                break;
            case DoorDirection.DoorLeft:
                diff = max.x - min.x;

                max.x = min.x;
                min.x -= diff;
                break;
            default:
                break;
        }

        // Générer la prochaine salle
        FindObjectOfType<LevelGenerator>().GenerateRoom(min, max);
    }
}
