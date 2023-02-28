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

    private List<GameObject> bases = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            bases.Add(GameObject.Find($"UnitBase {i + 1}"));
        }
    }
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
       
        //bases.Add(unitBase);
    }

    [Server]
    private void ServerHandleBaseDespawned(UnitBase unitBase)
    {
        var uBase = bases.FirstOrDefault(x => x.gameObject.name == unitBase.name);
        bases.Remove(uBase);
      
        if (bases.Count != 1) return;

        int winnerTeamId = bases[0].GetComponent<TeamNumber>().GetTeamNumber();

        RpcGameOver($"Team {winnerTeamId} won!");
       
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
