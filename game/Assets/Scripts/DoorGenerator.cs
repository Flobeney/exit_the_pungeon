using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorGenerator : MonoBehaviour
{
    // Constants
    private const int PERCENT_CHANCE_DOOR = 10;
    private const int SIZE_DOOR = 2;
    private const int MAX_DOOR_PER_WALL = 1;
    private const int MAX_ROOMS = 10;

    // Champs
    public GameObject DoorPrefab;

    // Champs privés
    private int _nbRooms = 1; // Init à 1 car la première salle est générée

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Nb rooms max : " + MAX_ROOMS);
    }

    /// <summary>
    /// Returns the size of the door
    /// </summary>
    /// <returns>The size of the door</returns>
    public int GetSizeDoor(){
        return SIZE_DOOR;
    }

    /// <summary>
    /// Generates the doors of the level
    /// </summary>
    /// <param name="nbDoor">Num of door of the current wall</param>
    /// <param name="i">Current pos on the wall</param>
    /// <param name="start">Start of the wall</param>
    /// <param name="limit">Limit of the wall</param>
    /// <param name="min">Min of the camera</param>
    /// <param name="max">Max of the camera</param>
    /// <param name="sizeTile">Size of the tile</param>
    /// <param name="isHorizontal">If the wall is horizontal</param>
    /// <param name="isLeft">If the wall is on the left</param>
    public bool SpawnDoor(int nbDoor, float i, float start, float limit, Vector3 min, Vector3 max, Vector3 sizeTile, bool isHorizontal, bool isLeft){
        // Générer une porte (évt.)
        if(
            nbDoor < MAX_DOOR_PER_WALL // Pas trop de portes par mur
            && _nbRooms < MAX_ROOMS // Pas trop de salles par niveau
            && MustGenerateDoor(i, start, limit)
        ){
            // Incrémenter le nb de portes
            nbDoor++;
            // Incrémenter le nb de salles totales
            _nbRooms++;

            // Générer la porte
            GenerateDoor(i, min, max, sizeTile, isHorizontal, isLeft);

            return true;
        }
        return false;
    }

    /// <summary>
    /// Defined whether to generate a door
    /// </summary>
    /// <param name="i">Current position</param>
    /// <param name="start">Start of the camera</param>
    /// <param name="limit">Limit of the camera</param>
    bool MustGenerateDoor(float i, float min, float max){
        // Porte pour changer de niveau
        if(i > min && i < max - SIZE_DOOR){ // S'assurer de pas être au 1er ou dernier tile
            // Si le % est atteint
            return Random.Range(0, 100) < PERCENT_CHANCE_DOOR;
        }
        return false;
    }

    /// <summary>
    /// Generates a door
    /// </summary>
    /// <param name="i">Current pos</param>
    /// <param name="min">Min of the camera</param>
    /// <param name="max">Max of the camera</param>
    /// <param name="sizeTile">Size of the tile</param>
    /// <param name="isHorizontal">If the wall is horizontal</param>
    /// <param name="isLeft">If the wall is on the left</param>
    void GenerateDoor(float i, Vector3 min, Vector3 max, Vector3 sizeTile, bool isHorizontal, bool isLeft){
        // Instancier la porte
        Vector3 posDoorLeft;
        Vector3 posDoorRight;
        if(isHorizontal){
            if(isLeft){ // Haut
                posDoorLeft = new Vector3((i-1) + (sizeTile.x / 2), max.y - (sizeTile.y / 2) + sizeTile.y, 0);
                posDoorRight = new Vector3((i+SIZE_DOOR+1) + (sizeTile.x / 2), max.y - (sizeTile.y / 2) + sizeTile.y, 0);
            }else{ // Bas
                posDoorLeft = new Vector3((i-1) + (sizeTile.x / 2), min.y + (sizeTile.y / 2) - sizeTile.y, 0);
                posDoorRight = new Vector3((i+SIZE_DOOR+1) + (sizeTile.x / 2), min.y + (sizeTile.y / 2) - sizeTile.y, 0);
            }
        }else{
            if(isLeft){ // Gauche
                posDoorLeft = new Vector3(min.x + (sizeTile.x / 2) - sizeTile.x, (i-1) + (sizeTile.y / 2), 0);
                posDoorRight = new Vector3(min.x + (sizeTile.x / 2) - sizeTile.x, (i+SIZE_DOOR+1) + (sizeTile.y / 2), 0);
            }else{ // Droite
                posDoorLeft = new Vector3(max.x - (sizeTile.x / 2) + sizeTile.x, (i-1) + (sizeTile.y / 2), 0);
                posDoorRight = new Vector3(max.x - (sizeTile.x / 2) + sizeTile.x, (i+SIZE_DOOR+1) + (sizeTile.y / 2), 0);
            }
        }
        // Générer le mur
        // Instantiate(TilePrefab, posDoorLeft, Quaternion.identity);
        // Instantiate(TilePrefab, posDoorRight, Quaternion.identity);

        // "Porte" pour générer la salle suivante
        // Changer la taille de la porte
        Vector3 sizeDoor = new Vector3(
            isHorizontal ? sizeTile.x * SIZE_DOOR : sizeTile.x,
            isHorizontal ? sizeTile.y : sizeTile.y * SIZE_DOOR,
            1
        );
        DoorPrefab.transform.localScale = sizeDoor;
        // Définir la position de la porte
        Vector3 posDoor = isHorizontal 
        ? new Vector3(
            (posDoorRight.x - posDoorLeft.x) + posDoorLeft.x - sizeDoor.x, 
            isLeft ? posDoorLeft.y - (sizeDoor.y/2) : posDoorLeft.y + (sizeDoor.y/2), 
            0
        ) 
        : new Vector3(
            isLeft ? posDoorLeft.x + (sizeDoor.x/2) : posDoorLeft.x - (sizeDoor.x/2), 
            (posDoorRight.y - posDoorLeft.y) + posDoorLeft.y - sizeDoor.y, 
            0
        );
        // Tag de la porte
        DoorPrefab.tag = isHorizontal
        ? (isLeft ? "DoorTop" : "DoorBottom")
        : (isLeft ? "DoorLeft" : "DoorRight");
        Instantiate(DoorPrefab, posDoor, Quaternion.identity);
    }
}
