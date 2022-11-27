using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    // Constants
    private const string MATERIALS_FOLDER = "LowlyPoly";

    // Champs
    public GameObject TilePrefab;

    // Start is called before the first frame update
    void Start()
    {
        GenerateWalls();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateWalls(){
        // Choose material
        Object[] materials = Resources.LoadAll(MATERIALS_FOLDER, typeof(Material));
        Material material = (Material)materials[Random.Range(0, materials.Length)];
        TilePrefab.GetComponent<Renderer>().material = material;

        // Get size of camera & tile
        Vector3 min = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 max = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));
        Vector3 sizeTile = TilePrefab.GetComponent<Renderer>().bounds.size;

        // Haut & bas
        for (float x = min.x; x < max.x; x += sizeTile.x){
            // Haut
            Instantiate(
                TilePrefab, 
                new Vector3(x + (sizeTile.x / 2), max.y - (sizeTile.y / 2), 0), 
                Quaternion.identity
            );
            // Bas
            Instantiate(
                TilePrefab, 
                new Vector3(x + (sizeTile.x / 2), min.y + (sizeTile.y / 2), 0), 
                Quaternion.identity
            );
        }
        // Gauche & droite
        for (float y = min.y; y < max.y; y += sizeTile.y){
            // Gauche
            Instantiate(
                TilePrefab, 
                new Vector3(min.x + (sizeTile.x / 2), y + (sizeTile.y / 2), 0), 
                Quaternion.identity
            );
            // Droite
            Instantiate(
                TilePrefab, 
                new Vector3(max.x - (sizeTile.x / 2), y + (sizeTile.y / 2), 0), 
                Quaternion.identity
            );
        }
    }
}
