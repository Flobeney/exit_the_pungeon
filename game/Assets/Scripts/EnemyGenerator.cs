using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    // Constants
    private const int MIN_ENEMIES = 3;
    private const int MAX_ENEMIES = 6;

    // Champs
    public GameObject EnemyPrefab;
    public GameObject Player;

    /// <summary>
    // Faire apparaître des ennemis
    /// </summary>
    /// <param name="min">Min of the camera</param>
    /// <param name="max">Max of the camera</param>
    public void SpawnEnemies(Vector3 min, Vector3 max){
        // Nombre d'ennemis
        int numEnemies = Random.Range(MIN_ENEMIES, MAX_ENEMIES);

        for (int i = 0; i < numEnemies; i++){
            // Créer l'ennemi
            Vector3 pos = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), 0);
            GameObject enemy = Instantiate(EnemyPrefab, pos, Quaternion.identity);
            // Set le joueur comme cible
            enemy.GetComponent<EnemyController>().Player = Player;
            enemy.GetComponent<EnemyBulletManager>().Player = Player;
        }
    }
}
