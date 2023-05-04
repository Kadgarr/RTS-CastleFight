using Mirror;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitPlayerPrefab = null;
    [SerializeField] private GameObject unitBasePrefab = null;
    [SerializeField] private GameOverHandler gameOverHandlerPrefab = null;

    private bool isGameInProgress=false;
    private string playerNamePath= "nameOption.txt";

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    private Callback callback;

    public List<RTSPlayer> Players { get; } = new List<RTSPlayer> ();

    #region Server

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (!isGameInProgress) return;
        conn.Disconnect();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

        Players.Remove(player);

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        Players.Clear();
        isGameInProgress = false;
    }

    public void StartGame()
    {
        if (Players.Count < 2) return;

        isGameInProgress = true;

        ServerChangeScene("Scene_Map_01"); 
    }

    #endregion

    #region Client
    #endregion

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        ClientOnDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
        Players.Clear();
    }
    
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();
    
        Players.Add(player);


        player.SetTeamColor(new Color(
            UnityEngine.Random.Range(0f,1f),
              UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f)
            ));

        player.SetPartyOwner(Players.Count == 1);
    }



    public override void OnServerSceneChanged(string sceneName)
    {
        if(SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);

            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);

            //спавним 2 базы
            foreach(RTSPlayer player in Players)
            {
                if (player.GetTeamNumber() == 1)
                {
                   GameObject unitBaseInstance = Instantiate(
                   unitBasePrefab,
                   new Vector3(-105.4f,1f,0f),
                   Quaternion.identity);

                   unitBaseInstance.name += $" {player.netId}";
                   NetworkServer.Spawn(unitBaseInstance, player.connectionToClient);

                    foreach(RTSPlayer player2 in Players)
                    {
                        if(player2.GetTeamNumber() == 2)
                        {
                            GameObject unitBase2Instance = Instantiate(
                            unitBasePrefab,
                            new Vector3(105.4f, 1f, 0f),
                            Quaternion.identity);

                            unitBase2Instance.name += $" {player2.netId}";
                            NetworkServer.Spawn(unitBase2Instance, player2.connectionToClient);
                            break;
                        }
                    }

                    break;
                }
            }
        }

        //спавним игроков
        foreach(RTSPlayer player in Players)
        {

            if(player.GetTeamNumber() == 1)
            {
                GameObject builderInstance = Instantiate(
                 unitPlayerPrefab,
                 new Vector3(-98f, 0f, 0f),
                 Quaternion.identity);

                builderInstance.name += $" {player.netId}";
                NetworkServer.Spawn(builderInstance, player.connectionToClient);
            }
            else
            {
                GameObject builderInstance = Instantiate(
                unitPlayerPrefab,
                new Vector3(98f, 0f, 0f),
                Quaternion.identity);

                builderInstance.name += $" {player.netId}";
                NetworkServer.Spawn(builderInstance, player.connectionToClient);
            }
           
        }
    }
}
