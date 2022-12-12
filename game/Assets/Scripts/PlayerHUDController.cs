using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class PlayerHUDController : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    private NetworkVariable<FixedString128Bytes> networkPlayerName = new NetworkVariable<FixedString128Bytes>("Player 0",
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        networkPlayerName.Value = "Player " + (OwnerClientId + 1);
        playerNameText.text = networkPlayerName.Value.ToString();
    }
}
