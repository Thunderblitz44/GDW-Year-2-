using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class Relayy : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TextMeshProUGUI joinCodeTxt;
    [SerializeField] UnityTransport relayTransport;
    [SerializeField] UnityTransport unityTransport;
    
    async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (AuthenticationException e)
        {
            Debug.LogError(e);
        }
    }

    public async void CreateRelay()
    {
        NetworkManager.Singleton.NetworkConfig.NetworkTransport = relayTransport;

        try
        {
            Allocation alloc = await RelayService.Instance.CreateAllocationAsync(1);
            RelayServerData relayServerData = new RelayServerData(alloc, "dtls");
            relayTransport.SetRelayServerData(relayServerData);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);
            NetworkManager.Singleton.StartHost();

            joinCodeTxt.text = "Join Code: " + joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinRelay()
    {
        NetworkManager.Singleton.NetworkConfig.NetworkTransport = relayTransport;
        try
        {
            JoinAllocation joinAlloc = await RelayService.Instance.JoinAllocationAsync(inputField.text);
            RelayServerData relayServerData = new RelayServerData(joinAlloc, "dtls");
            relayTransport.SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void StartLanHost()
    {
        NetworkManager.Singleton.NetworkConfig.NetworkTransport = unityTransport;
        NetworkManager.Singleton.StartHost();
    }

    public void StartLanClient()
    {
        NetworkManager.Singleton.NetworkConfig.NetworkTransport = unityTransport;
        NetworkManager.Singleton.StartClient();
    }

    public void StopRelay()
    {

    }

    public void StopHostLan()
    {
    }
}
