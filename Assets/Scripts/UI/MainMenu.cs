using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartServer()
    {
        SceneManager.LoadScene(1);
        NetworkManager.Singleton.StartServer();
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("Movement", LoadSceneMode.Single);
        //NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    void HandleServerStarted()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Movement", LoadSceneMode.Single);
    }
}
