using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] GameObject Player1;
    [SerializeField] GameObject Player2;

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (ulong cliendId) =>
        {
            Debug.Log("player " + cliendId + " joined!");
            ShowPlayerServerRpc(cliendId);
        };
        NetworkManager.Singleton.OnClientDisconnectCallback += (ulong cliendId) =>
        {
            Debug.Log("player " + cliendId + " left!");
            HidePlayerServerRpc(cliendId);
        };

        NetworkManager.Singleton.OnClientStarted += () =>
        {
            Debug.Log("client started");
        };
        NetworkManager.Singleton.OnClientStopped += (bool isStopped) =>
        {
            Debug.Log("client stopped");
        };

        if (!IsServer) return;

        NetworkManager.Singleton.OnServerStarted += () =>
        {
            Debug.Log("server started");
        };
        NetworkManager.Singleton.OnServerStopped += (bool isStopped) =>
        {
            Debug.Log("server stopped");
        };
    }

    [ServerRpc(RequireOwnership = false)]
    void ShowPlayerServerRpc(ulong playerId)
    {
        if (playerId == 0) { Player1.SetActive(true); }
        if (playerId == 1) { Player2.SetActive(true); }
    }

    [ServerRpc(RequireOwnership = false)]
    void HidePlayerServerRpc(ulong playerId)
    {
        if (playerId == 0) { Player1.SetActive(false); }
        if (playerId == 1) { Player2.SetActive(false); }
    }

    [ServerRpc(RequireOwnership =false)]
    public void DisconnectServerRpc()
    {
        ulong myId = NetworkManager.Singleton.LocalClientId;
        NetworkManager.Singleton.DisconnectClient(myId,"disconnected");
        NetworkManager.SendMessage("CLOSE",null, SendMessageOptions.DontRequireReceiver);
    }
}
