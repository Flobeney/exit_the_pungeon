using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    // Constants
    private const string MATERIALS_WALL_FOLDER = "LowlyPoly";
    private const string DUNGEON_2D_FOLDER = "Dungeon-2D-PixelArt-Tileset";
    private const float Z_INDEX_WALL = -5f;
    private const float Z_INDEX_FLOOR = 0f;

    // Champs
    public GameObject TilePrefab;
    public GameObject FloorPrefab;

    // Champs privés
    /// Cette liste se compose de la manière suivante
    /// Key Item1 : coordonnées x de la salle
    /// Key Item2 : coordonnées y de la salle
    /// Value : coordonnées de la caméra
    private IDictionary<(int, int), Vector3> _rooms = new Dictionary<(int, int), Vector3>();
    private Vector3 _roomSize;
    private int _x;
    private int _y;

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

    public int GetX(){
        return _x;
    }
    public int GetY(){
        return _y;
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
        _x = (int)System.Math.Round(nextCamPosition.x / _roomSize.x);
        _y = (int)System.Math.Round(nextCamPosition.y / _roomSize.y);

        // Walls to not generate
        List<DoorDirection> wallsToNotGenerate = GetWallsToNotGenerate();

        // Is room already generated ?
        if(_rooms.ContainsKey((_x, _y))){
            // Room already generated
            Debug.Log("Room already generated");
            Camera.main.transform.position = _rooms[(_x, _y)];
            return;
        }else{
            // Room not generated
            Debug.Log("Room not generated");
            // Add room to list
            _rooms.Add((_x, _y), nextCamPosition);
        }

        // Choose material
        Object[] materials = Resources.LoadAll(MATERIALS_WALL_FOLDER, typeof(Material));
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
        // GenerateFloor(materials, min, max);

        // Enemies (si pas 1ère salle)
        if(_rooms.Count > 1){
            GameObject.Find("EnemyGenerator").GetComponent<EnemyGenerator>().SpawnEnemies(min, max);
        }

        // Move camera
        Camera.main.transform.position = nextCamPosition;
    }

    /// <summary>
    /// Compute the walls to not generate
    /// </summary>
    /// <returns>List of walls to not generate</returns>
    List<DoorDirection> GetWallsToNotGenerate(){
        List<DoorDirection> wallsToNotGenerate = new List<DoorDirection>();

        // Check si la salle de gauche existe
        if(_rooms.ContainsKey((_x - 1, _y))){
            wallsToNotGenerate.Add(DoorDirection.DoorLeft);
        }
        // Check si la salle de droite existe
        if(_rooms.ContainsKey((_x + 1, _y))){
            wallsToNotGenerate.Add(DoorDirection.DoorRight);
        }
        // Check si la salle du haut existe
        if(_rooms.ContainsKey((_x, _y + 1))){
            wallsToNotGenerate.Add(DoorDirection.DoorTop);
        }
        // Check si la salle du bas existe
        if(_rooms.ContainsKey((_x, _y - 1))){
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
        // Définir quelles variables utiliser suivant si c'est haut/bas ou gauche/droite
        float start = isHorizontal ? min.x - (sizeTile.x/2) : min.y - (sizeTile.y/2);
        float limit = isHorizontal ? max.x + (sizeTile.x/2) : max.y + (sizeTile.y/2);
        float incr = isHorizontal ? sizeTile.x : sizeTile.y;

        // Boucle
        for (float i = start; i < limit; i += incr){
            DoorGenerator doorGenerator = GameObject.Find("DoorGenerator").GetComponent<DoorGenerator>();
            // Générer une porte (évt.)
            if(doorGenerator.SpawnDoor(nbDoor, i, start, limit, min, max, sizeTile, isHorizontal, isLeft)){
                // Incrémenter le nb de portes
                nbDoor++;
                // Sauter 2 cases
                i += incr * doorGenerator.GetSizeDoor();
                continue;
            }
            // Définir la position en fonction de si c'est haut/bas ou gauche/droite
            Vector3 pos;
            if(isHorizontal){
                pos = isLeft 
                ? new Vector3(i + (sizeTile.x / 2), max.y, Z_INDEX_WALL) 
                : new Vector3(i + (sizeTile.x / 2), min.y, Z_INDEX_WALL);
            }else{
                pos = isLeft 
                ? new Vector3(min.x, i + (sizeTile.y / 2), Z_INDEX_WALL) 
                : new Vector3(max.x, i + (sizeTile.y / 2), Z_INDEX_WALL);
            }
            // Générer le mur
            Instantiate(TilePrefab, pos, Quaternion.identity);
        }
    }

    /// <summary>
    /// Generates the floor of the level
    /// </summary>
    /// <param name="materials">Materials available</param>
    /// <param name="min">Min of the camera</param>
    /// <param name="max">Max of the camera</param>
    void GenerateFloor(Object[] materials, Vector3 min, Vector3 max){
        // List floor sprites
        Texture2D texture = Resources.Load<Texture2D>(DUNGEON_2D_FOLDER);
        Sprite[] sprites = Resources.LoadAll<Sprite>(texture.name).Where(s => s.name.Contains("Floor")).ToArray();

        // Get size of floor
        Vector3 sizeFloor = FloorPrefab.GetComponent<Renderer>().bounds.size;

        // Boucle
        for (float x = min.x - (sizeFloor.x/2); x < max.x + (sizeFloor.x/2); x += sizeFloor.x){
            for (float y = min.y - (sizeFloor.y/2); y < max.y + (sizeFloor.y/2); y += sizeFloor.y){
                // Choose sprite
                FloorPrefab.GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];

                // Générer le mur
                Instantiate(
                    FloorPrefab, 
                    new Vector3(x + (sizeFloor.x / 2), y + (sizeFloor.y / 2), Z_INDEX_FLOOR), 
                    Quaternion.identity
                );
            }
        }
    }
}
