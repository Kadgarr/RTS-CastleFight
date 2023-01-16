using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private int resourcesCost = 10;
    [SerializeField] private Health health = null; 
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private UnityEvent onDeselected = null;
    [SerializeField] private UnitMovement unitMovement = null;

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;

    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }

    public Targeter GetTargeter()
    {
        return targeter;
    }

    public int GetResourceCost()
    {
        return resourcesCost;
    }
    #region Server

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleOnDie;
        ServerOnUnitSpawned?.Invoke(this);

    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleOnDie;
        ServerOnUnitDespawned?.Invoke(this);
    }

    [Server]
    private void ServerHandleOnDie()
    {
        NetworkServer.Destroy(gameObject);
    }
    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!hasAuthority) return;

        AuthorityOnUnitDespawned?.Invoke(this);
    }

    public void Select()
    {
        if (!hasAuthority) return;
        onSelected?.Invoke();
    }

    public void Deselect()
    {
        if (!hasAuthority) return;
        onDeselected?.Invoke(); 
    }
    #endregion
}
