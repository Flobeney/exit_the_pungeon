using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyGenerator : NetworkBehaviour
{
    // Constants
    private const int MIN_ENEMIES = 3;
    private const int MAX_ENEMIES = 6;

    // Champs
    public GameObject EnemyPrefab;

    // Champs privés
    private int _nbEnemies;
    private GameObject[] _players;

    // Start is called before the first frame update
    void Start(){
        if(IsHost || IsServer){
            // Récupérer les joueurs de la scène (online)
            _players = GameObject.FindGameObjectsWithTag("Player");
        }
    }

    /// <summary>
    // Faire apparaître des ennemis
    /// </summary>
    /// <param name="min">Min of the camera</param>
    /// <param name="max">Max of the camera</param>
    public void SpawnEnemies(Vector3 min, Vector3 max){
        SpawnEnemiesServerRpc(min, max);
    }

    [ServerRpc]
    void SpawnEnemiesServerRpc(Vector3 min, Vector3 max){
        // Nombre d'ennemis
        _nbEnemies = Random.Range(MIN_ENEMIES, MAX_ENEMIES);

        for (int i = 0; i < _nbEnemies; i++){
            // Créer l'ennemi
            Vector3 pos = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), 0);
            GameObject enemy = Instantiate(EnemyPrefab, pos, Quaternion.identity);
            // Set le joueur comme cible
            enemy.GetComponent<EnemyBulletManager>().Players = new List<GameObject>(_players);
            // Spawn on the network
            enemy.GetComponent<NetworkObject>().Spawn();
        }
    }

    /// <summary>
    /// Called when an enemy is destroyed
    /// </summary>
    public void EnemyDestroyed(){
        _nbEnemies--;

        // Si plus d'ennemis, on ouvre les portes
        if(_nbEnemies == 0){
            Debug.Log("All enemies destroyed, opening doors");
            OpenDoorsServerRpc();
        }
    }

    [ServerRpc]
    void OpenDoorsServerRpc(){
        GameObject.Find("DoorGenerator").GetComponent<DoorGenerator>().OpenDoorsServerRpc();
    }
}
