using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
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
            // Get size of camera
            Vector3 min = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
            Vector3 max = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));

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

            // Ne pas générer de là où on vient
            FindObjectOfType<LevelGenerator>().GenerateRoom(min, max, doorDirection);
            // Détruire la porte qu'on vient de passer
            Destroy(this.gameObject);
        }

    }
}
