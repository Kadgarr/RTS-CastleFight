using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [SyncVar(hook =nameof(HandleHealtUpdated))]
    private int currentHealth;

    public event Action ServerOnDie;
    public static event Action<int> ServerOnPlayerDie;
    public event Action<int, int> ClientOnHealthUpdated;

    #region Server

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
        UnitBase.ServerOnPlayerDie+=ServerOnHandlePlayerDie;
    }
    public override void OnStopServer()
    {
        UnitBase.ServerOnPlayerDie -= ServerOnHandlePlayerDie;
    }

    [Server]
    private void ServerOnHandlePlayerDie(int connectionId)
    {
        if(connectionToClient.connectionId!=connectionId) return;

        DealDamage(currentHealth);

       // ServerOnPlayerDie?.Invoke(connectionId);
    }



    [Server]
    public void DealDamage(int damageAmount)
    {
        if (currentHealth == 0) return;

        currentHealth= Mathf.Max(currentHealth- damageAmount,0);

        if (currentHealth != 0) return;

        ServerOnDie?.Invoke();

       
    }

    #endregion

    #region Client

    private void HandleHealtUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    public int GetHealth()
    {
        return maxHealth;
    }
    #endregion
}
