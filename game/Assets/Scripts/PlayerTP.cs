using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerTP : NetworkBehaviour
{
    // Fonction pour déplacer les joueurs d'une salle à l'autre
    [ServerRpc]
    public void MovePlayerServerRpc(Vector3 currentPos){
        // Récupérer les joueurs de la scène (online)
        GameObject[] _players = GameObject.FindGameObjectsWithTag("Player");

        // Parcourir les joueurs
        foreach (GameObject player in _players){
            Debug.Log("Before : " + player.transform.position);
            // Si le joueur est null (détruit), continuer
            if(player == null) continue;
            // Si la position est celle du joueur courant, continuer
            if(player.transform.position == currentPos) continue;
            // Déplacer le joueur
            player.transform.position = new Vector3(currentPos.x, currentPos.y, player.transform.position.z);
            Debug.Log("After : " + player.transform.position);
        }
    }
}
