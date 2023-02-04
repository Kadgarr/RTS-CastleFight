using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private TMP_InputField addressInput = null;
    [SerializeField] private Button joinButton = null;
    protected Callback<LobbyEnter_t> lobbyEntered;


    private void OnEnable()
    {
        RTSNetworkManager.ClientOnConnected += HandleClientConnected;
        RTSNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
    }
    private void OnDisable()
    {
        RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
        RTSNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
    }
    public void Join()
    {
        string address = addressInput.text;
        NetworkManager.singleton.networkAddress = address;
        NetworkManager.singleton.StartClient();

        joinButton.interactable = false;
        //lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEnter);

    }
    private void OnLobbyEnter(LobbyEnter_t callback)
    {
        if (NetworkServer.active) return;

        string address = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            addressInput.text);

        NetworkManager.singleton.networkAddress = address;
        NetworkManager.singleton.StartClient();

        joinButton.interactable = false;
    }
    private void HandleClientConnected()
    {
        joinButton.interactable = true;

        this.gameObject.SetActive(false);
        landingPagePanel.SetActive(false);


    }

    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }
}
