using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    // Constants
    private const string MATERIALS_FOLDER = "LowlyPoly";
    private const int PERCENT_CHANCE_DOOR = 10;
    private const int SIZE_DOOR = 2;
    private const int MAX_DOOR_PER_WALL = 1;

    // Champs
    public GameObject TilePrefab;
    public GameObject DoorPrefab;

    // Champs privés
    /// Cette liste se compose de la manière suivante
    /// Key Item1 : coordonnées x de la salle
    /// Key Item2 : coordonnées y de la salle
    /// Value : coordonnées de la caméra
    private IDictionary<(int, int), Vector3> _rooms = new Dictionary<(int, int), Vector3>();
    private Vector3 _roomSize;

    // Start is called before the first frame update
    void Start()
    {
        // Get size of camera
        Vector3 min = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 max = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));

        // Get size of room
        _roomSize = new Vector3(max.x - min.x, max.y - min.y, 0);

        GenerateRoom(min, max);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Generates a room
    /// </summary>
    /// <param name="min">Min of the camera</param>
    /// <param name="max">Max of the camera</param>
    public void GenerateRoom(Vector3 min, Vector3 max){
        // Compute new camera position
        Vector3 nextCamPosition = new Vector3((max.x + min.x) / 2, (max.y + min.y) / 2, Camera.main.transform.position.z);

        // Compute the xy position of the room
        // Faire arrondi
        int x = (int)System.Math.Round(nextCamPosition.x / _roomSize.x);
        int y = (int)System.Math.Round(nextCamPosition.y / _roomSize.y);

        // Walls to not generate
        List<DoorDirection> wallsToNotGenerate = GetWallsToNotGenerate(x, y);

        // Is room already generated ?
        if(_rooms.ContainsKey((x, y))){
            // Room already generated
            Debug.Log("Room already generated");
            Camera.main.transform.position = _rooms[(x, y)];
            return;
        }else{
            // Room not generated
            Debug.Log("Room not generated");
            // Add room to list
            _rooms.Add((x, y), nextCamPosition);
        }

        // Choose material
        Object[] materials = Resources.LoadAll(MATERIALS_FOLDER, typeof(Material));
        Material material = (Material)materials[Random.Range(0, materials.Length)];
        TilePrefab.GetComponent<Renderer>().material = material;

        // Get size of tile
        Vector3 sizeTile = TilePrefab.GetComponent<Renderer>().bounds.size;

        // Haut
        if(!wallsToNotGenerate.Contains(DoorDirection.DoorTop)) GenerateWall(min, max, sizeTile, true, true);
        // Bas
        if(!wallsToNotGenerate.Contains(DoorDirection.DoorBottom)) GenerateWall(min, max, sizeTile, true, false);
        // Gauche
        if(!wallsToNotGenerate.Contains(DoorDirection.DoorLeft)) GenerateWall(min, max, sizeTile, false, true);
        // Droite
        if(!wallsToNotGenerate.Contains(DoorDirection.DoorRight)) GenerateWall(min, max, sizeTile, false, false);

        // Floor
        // GenerateFloor(materials, min, max, sizeTile);

        // Move camera
        Camera.main.transform.position = nextCamPosition;
    }

    /// <summary>
    /// Compute the walls to not generate
    /// </summary>
    /// <param name="doorDirection">Direction of the door from which you come</param>
    /// <param name"x">x position of the room</param>
    /// <param name="y">y position of the room</param>
    /// <returns>List of walls to not generate</returns>
    List<DoorDirection> GetWallsToNotGenerate(int x, int y){
        List<DoorDirection> wallsToNotGenerate = new List<DoorDirection>();

        // Check si la salle de gauche existe
        if(_rooms.ContainsKey((x - 1, y))){
            wallsToNotGenerate.Add(DoorDirection.DoorLeft);
        }
        // Check si la salle de droite existe
        if(_rooms.ContainsKey((x + 1, y))){
            wallsToNotGenerate.Add(DoorDirection.DoorRight);
        }
        // Check si la salle du haut existe
        if(_rooms.ContainsKey((x, y + 1))){
            wallsToNotGenerate.Add(DoorDirection.DoorTop);
        }
        // Check si la salle du bas existe
        if(_rooms.ContainsKey((x, y - 1))){
            wallsToNotGenerate.Add(DoorDirection.DoorBottom);
        }

        return wallsToNotGenerate;
    }

    // public bool IsRoomAlreadyGenerated(Vector3 min, Vector3 max){
    //     return _rooms.ContainsKey((min, max));
    // }

    /// <summary>
    /// Generates the walls of the level
    /// </summary>
    /// <param name="min">Min of the camera</param>
    /// <param name="max">Max of the camera</param>
    /// <param name="sizeTile">Size of the tile</param>
    /// <param name="isHorizontal">If the wall is horizontal</param>
    /// <param name="isLeft">If the wall is on the left</param>
    void GenerateWall(Vector3 min, Vector3 max, Vector3 sizeTile, bool isHorizontal, bool isLeft){
        // Nb de portes sur le mur actuel
        int nbDoor = 0;
        float lastDoorPosition = 0;
        // Définir quelles variables utiliser suivant si c'est haut/bas ou gauche/droite
        float start = isHorizontal ? min.x - (sizeTile.x/2) : min.y - (sizeTile.y/2);
        float limit = isHorizontal ? max.x + (sizeTile.x/2) : max.y + (sizeTile.y/2);
        // float start = isHorizontal ? min.x : min.y;
        // float limit = isHorizontal ? max.x : max.y;
        float incr = isHorizontal ? sizeTile.x : sizeTile.y;

        // Boucle
        for (float i = start; i < limit; i += incr){
            // Générer une porte (évt.)
            if(
                nbDoor < MAX_DOOR_PER_WALL // Pas trop de portes par mur
                && MustGenerateDoor(i, start, limit)
            ){
                // Incrémenter le nb de portes
                nbDoor++;

                // Générer la porte
                GenerateDoor(i, min, max, sizeTile, isHorizontal, isLeft);

                // Sauter 2 cases
                i += incr * SIZE_DOOR;
                // Définir la position de la dernière porte
                lastDoorPosition = i;
                continue;
            }
            // Définir la position en fonction de si c'est haut/bas ou gauche/droite
            Vector3 pos;
            if(isHorizontal){
                pos = isLeft 
                ? new Vector3(i + (sizeTile.x / 2), max.y, 0) 
                : new Vector3(i + (sizeTile.x / 2), min.y, 0);
            }else{
                pos = isLeft 
                ? new Vector3(min.x, i + (sizeTile.y / 2), 0) 
                : new Vector3(max.x, i + (sizeTile.y / 2), 0);
            }
            // Générer le mur
            Instantiate(TilePrefab, pos, Quaternion.identity);
        }
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

    /// <summary>
    /// Generates the floor of the level
    /// </summary>
    /// <param name="materials">Materials available</param>
    /// <param name="min">Min of the camera</param>
    /// <param name="max">Max of the camera</param>
    /// <param name="sizeTile">Size of the tile</param>
    void GenerateFloor(Object[] materials, Vector3 min, Vector3 max, Vector3 sizeTile){
        // Choose material
        Material material = (Material)materials[Random.Range(0, materials.Length)];
        TilePrefab.GetComponent<Renderer>().material = material;

        // Boucle
        for (float x = min.x + sizeTile.x; x < max.x - sizeTile.x; x += sizeTile.x){
            for (float y = min.y + sizeTile.y; y < max.y - sizeTile.y; y += sizeTile.y){
                // Générer le mur
                Instantiate(
                    TilePrefab, 
                    new Vector3(x + + (sizeTile.x / 2), y + + (sizeTile.y / 2), 0), 
                    Quaternion.identity
                );
            }
        }
    }
}
