using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private Renderer colorRenderer = new Renderer();

    public static event Action<int> ServerOnPlayerDie;
    public static event Action<UnitBase> ServerOnBaseSpawned;
    public static event Action<UnitBase> ServerOnBaseDespawned;

   

    #region Server

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandelDie;

        ServerOnBaseSpawned?.Invoke(this);
    }

  
    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandelDie;
    }

    private void ServerHandelDie()
    {
        ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);

        NetworkServer.Destroy(gameObject);
    }

    private void OnDestroy()
    {
        ServerOnBaseDespawned?.Invoke(this);
    }
    #endregion



    #region Client

    #endregion
}
