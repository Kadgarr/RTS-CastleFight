using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField]private GameObject lobbyUI=null;
    [SerializeField] private Button startGameButton = null;
    [SerializeField] private TMP_Text[] playerNameTexts=new TMP_Text[2];
    [SerializeField] private TMP_Text[] teamField = new TMP_Text[2];

    private RTSPlayer player;

    private void Start()
    {
        RTSNetworkManager.ClientOnConnected += HandleClientConnected;
        RTSPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        RTSPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
        RTSPlayer.ClientOnTeamInfoUpdated += HandleChangeTeam;
    }

    private void OnDestroy()
    {
        RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
        RTSPlayer.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
        RTSPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
        RTSPlayer.ClientOnTeamInfoUpdated -= HandleChangeTeam;
    }

    private void ClientHandleInfoUpdated()
    {
        List<RTSPlayer> players = ((RTSNetworkManager)NetworkManager.singleton).Players;

        for (int i = 0; i < players.Count; i++)
        {
            playerNameTexts[i].text = players[i].GetDisplayName();

            //if (players[i].GetTeamNumber() == 0)
            //{
            //    if (i == 0 || i == 1)
            //        players[i].CmdSetTeamNumber(1);
            //    else
            //        players[i].CmdSetTeamNumber(2);

            //}
            if (players[i].GetTeamNumber() == 1)
                teamField[i].text = "Team 1";
            else
                teamField[i].text = "Team 2";

        }

        for (int i = players.Count; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting for player...";
        }

        startGameButton.interactable = players.Count > 1;
    }

    public void ChangeToFirstTeam()
    {
        //получаем подключение к игроку, который меняет команду
        if (player == null)
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
      
        player.CmdSetTeamNumber(1);
      
    }

    public void ChangeToSecondTeam()
    {
        //получаем подключение к игроку, который меняет команду
        if (player == null)
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

        player.CmdSetTeamNumber(2);
       
    }

    private void HandleChangeTeam()
    {
        List<RTSPlayer> players = ((RTSNetworkManager)NetworkManager.singleton).Players;
        
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

        for(int i = 0; i < players.Count; i++)
        {
            if (players[i].GetTeamNumber() == 1)
                teamField[i].text = "Team 1";
            else
                teamField[i].text = "Team 2";
        }
       // Debug.LogError($"teamNumber: "+teamNumber);
        //if (teamNumber == 1)
        //{
        //    for (int i = 0; i < playerNameTexts.Length; i++)
        //    {
        //        if (playerNameTexts[i].text == player.GetDisplayName())
        //        {
        //            teamField[i].text = "Team 1";
        //        }
        //    }
        //    //bool check = true;
        //    ////if (player.GetTeamNumber() == 1) return;
        //    ////проход по текстовым полям 2-й команды в поисках имени игрока
        //    //for (int i = 2; i < 4; i++)
        //    //{

        //    //    if (playerNameTexts[i].text == player.GetDisplayName())
        //    //    {
        //    //        for (int j = 0; j < 2; j++)
        //    //        {
        //    //            //если есть свободное место меняем команду
        //    //            if (playerNameTexts[j].text == "Waiting for player...")
        //    //            {
        //    //                playerNameTexts[i].text = "Waiting for player...";
        //    //                playerNameTexts[j].text = player.GetDisplayName();
        //    //                break;
        //    //            }

        //    //        }
        //    //        check = false;
        //    //        break;
        //    //    }
        //    //    if (!check) break;
        //    //}

        //}
        //else if (teamNumber == 2)
        //{
        //    for (int i = 0; i < playerNameTexts.Length; i++)
        //    {
        //        if (playerNameTexts[i].text == player.GetDisplayName())
        //        {
        //            teamField[i].text = "Team 2";
        //        }
        //    }
        //    //bool check = true;
        //    ////if (player.GetTeamNumber() == 2) return;
        //    ////проход по текстовым полям 1-й команды в поисках имени игрока
        //    //for (int i = 0; i < 2; i++)
        //    //{

        //    //    if (playerNameTexts[i].text == player.GetDisplayName() && player.netId == playerNetId)
        //    //    {
        //    //        for (int j = 2; j < 4; j++)
        //    //        {
        //    //            //если есть свободное место меняем команду
        //    //            if (playerNameTexts[j].text == "Waiting for player...")
        //    //            {
        //    //                playerNameTexts[i].text = "Waiting for player...";
        //    //                playerNameTexts[j].text = player.GetDisplayName();

        //    //                break;
        //    //            }

        //    //        }
        //    //        check = false;
        //    //        break;

        //    //    }

        //    //    if (!check) break;
        //    //}
        //}

    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }
    public void StartGame()
    {
        NetworkClient.connection.identity.GetComponent<RTSPlayer>().CmdStartGame();
    }

    private void HandleClientConnected()
    {
        lobbyUI.SetActive(true);
    }

    public void LeaveLobby()
    {
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
            SceneManager.LoadScene(0);
        }
    }
}
