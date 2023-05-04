using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;


public class Health : NetworkBehaviour
{
    public enum TypeOfArmor
    {
        Heavy = 0,
        Medium = 1,
        Light = 2,
        Divine = 3,
        Hero = 4,
        Fortified = 5,
        Unarmored = 6
    }

    [Header("Heal and Armor Settings - ХП и тип брони")]
    [SerializeField] private TypeOfArmor typeOfArmor = new TypeOfArmor();
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int levelOfArmor = 0;

    [Header("Chance of miss (in percentage)")]
    [SerializeField] private int miss = 0;

    [Header("Cost for killing")]
    [SerializeField] private int price;

    [SyncVar(hook =nameof(HandleHealtUpdated))]
    private int currentHealth;

    private float dealDamage;

    public event Action ServerOnDie;
    public static event Action<int> ServerOnPlayerDie;
    public event Action<float> GetDamage;
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

        ServerOnPlayerDie?.Invoke(connectionId);
    }



    [Server]
    public void DealDamage(float damageAmount)
    {
        //Debug.Log($"Damage BEFORE: {damageAmount} object: {gameObject.name}");

        if (currentHealth == 0) return;

        dealDamage = damageAmount; 

        GetDamage?.Invoke(damageAmount);

        if(damageAmount!=dealDamage)
            damageAmount = dealDamage;

        //Debug.Log($"Damage AFTER: {damageAmount}");

        currentHealth = Mathf.Max(currentHealth - (int)damageAmount,0);

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

    public TypeOfArmor GetTypeOfArmor()
    {
        return typeOfArmor;
    }

    public int GetChanceOfMiss()
    {
        return miss;
    }
    public int GetPriceForKilling()
    {
        return price;
    } 
    public int GetLevelOfArmor()
    {
        return levelOfArmor;
    }

    public void SetLevelOfArmor(int number)
    {
        levelOfArmor=number;
    }

    [Command]
    public void SetDamage(float dealDamage)
    {
        this.dealDamage = dealDamage;
    }

    [Command]
    public void SetChanceOfMiss(float dealDamage)
    {
        this.dealDamage = dealDamage;
    }
}
