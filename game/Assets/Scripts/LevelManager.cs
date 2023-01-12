using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class LevelManager : NetworkBehaviour
{
    // Constantes
    const string NAME_SCENE_GAME = "LevelGenerator";

    // Chargement de la scène de jeu
    [ServerRpc]
    public void LoadSceneServerRpc(){
        NetworkManager.SceneManager.LoadScene(NAME_SCENE_GAME, LoadSceneMode.Additive);
    }
}
