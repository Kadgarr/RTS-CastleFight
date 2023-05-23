using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{

    public static event Action ServerOnGameOver;
    public static event Action<string> ClientOnGameOver;

    private List<UnitBase> bases = new List<UnitBase>();

    #region Server

    public override void OnStartServer()
    {
        UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned += ServerHandleBaseDespawned;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
    }

    [Server]
    private void ServerHandleBaseSpawned(UnitBase unitBase)
    {
        bases.Add(unitBase);
        Debug.Log($"Count of bases: {bases.Count}");
    }

    [Server]
    private void ServerHandleBaseDespawned(UnitBase unitBase)
    {
       
        bases.Remove(unitBase);
      
        if (bases.Count != 1) return;

        int winnerTeamId = bases[0].GetComponent<NetworkIdentity>().connectionToClient.identity.GetComponent<RTSPlayer>().GetTeamNumber();

        RpcGameOver($"Team {winnerTeamId}");
       
        ServerOnGameOver?.Invoke();
    }
    #endregion

    #region Client

    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }

    #endregion
}
