using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    // Constants
    private const string MATERIALS_FOLDER = "LowlyPoly";
    private const int PERCENT_CHANCE_DOOR = 10;
    private const int SIZE_DOOR = 2;
    private const int MAX_DOOR_PER_WALL = 2;
    private const int SPACE_BETWEEN_DOOR = 3;

    // Champs
    public GameObject TilePrefab;

    // Start is called before the first frame update
    void Start()
    {
        GenerateRoom();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Generates a room
    /// </summary>
    void GenerateRoom(){
        // Choose material
        Object[] materials = Resources.LoadAll(MATERIALS_FOLDER, typeof(Material));
        Material material = (Material)materials[Random.Range(0, materials.Length)];
        TilePrefab.GetComponent<Renderer>().material = material;
        Debug.Log(material.name);

        // Get size of camera & tile
        Vector3 min = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 max = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));
        Vector3 sizeTile = TilePrefab.GetComponent<Renderer>().bounds.size;

        // Haut
        GenerateWall(min, max, sizeTile, true, true);
        // Bas
        GenerateWall(min, max, sizeTile, true, false);
        // Gauche
        GenerateWall(min, max, sizeTile, false, true);
        // Droite
        GenerateWall(min, max, sizeTile, false, false);

        // Floor
        // GenerateFloor(materials, min, max, sizeTile);
    }

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
        float start = isHorizontal ? min.x : min.y;
        float limit = isHorizontal ? max.x : max.y;
        float incr = isHorizontal ? sizeTile.x : sizeTile.y;

        // Boucle
        for (float i = start; i < limit; i += incr){
            // Générer une porte (évt.)
            if(
                nbDoor < MAX_DOOR_PER_WALL // Pas trop de portes par mur
                && i - lastDoorPosition > SPACE_BETWEEN_DOOR * incr // Pas des portes trop proches
                && GenerateDoor(i, start, limit)
            ){
                // Incrémenter le nb de portes
                nbDoor++;
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
                ? new Vector3(i + (sizeTile.x / 2), max.y - (sizeTile.y / 2), 0) 
                : new Vector3(i + (sizeTile.x / 2), min.y + (sizeTile.y / 2), 0);
            }else{
                pos = isLeft 
                ? new Vector3(min.x + (sizeTile.x / 2), i + (sizeTile.y / 2), 0) 
                : new Vector3(max.x - (sizeTile.x / 2), i + (sizeTile.y / 2), 0);
            }
            // Générer le mur
            Instantiate(TilePrefab, pos, Quaternion.identity);
        }
    }

    /// <summary>
    /// Generates a door
    /// </summary>
    /// <param name="i">Current position</param>
    /// <param name="start">Start of the camera</param>
    /// <param name="limit">Limit of the camera</param>
    bool GenerateDoor(float i, float min, float max){
        // Porte pour changer de niveau
        if(i > min && i < max - SIZE_DOOR){ // S'assurer de pas être au 1er ou dernier tile
            // Si le % est atteint
            return Random.Range(0, 100) < PERCENT_CHANCE_DOOR;
        }
        return false;
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
        Debug.Log(material.name);
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
