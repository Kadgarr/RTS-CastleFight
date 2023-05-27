using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private Health health = null; 
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private GameObject infoUnitCanvas = null;
    [SerializeField] private GameObject buildMenuCanvas = null;

    [Header("Corpse prefab")]
    [SerializeField] private GameObject corpse = null;

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;


    [Header("Type of Unit - air or land")]
    [SerializeField] private bool isFlyingOnly;

    [Header("Can attack air")]
    [SerializeField] private bool isGroundOnly;

    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;

    private bool activeCanvasInfo;
    
    public bool GetIsFlyingOnly()
    {
       return isFlyingOnly;
    }

    public bool GetIsGroundOnly()
    {
        return isGroundOnly;
    }

    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }

    public Targeter GetTargeter()
    {
        return targeter;
    }

    //public int GetResourceCost()
    //{
    //    return resourcesCost;
    //}
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
        GameObject unitInstance = null;

        unitInstance = Instantiate(corpse, this.gameObject.transform.position, this.gameObject.transform.rotation);

        NetworkServer.Spawn(unitInstance, connectionToClient);

        NetworkServer.Destroy(gameObject);
        
    }
    #endregion

    #region Client

    public void SetActiveCanvasInfo(bool stateCanvas)
    {
        if (!stateCanvas)
        {
            infoUnitCanvas.SetActive(false);
            
            if(buildMenuCanvas!=null)
                buildMenuCanvas.SetActive(false);
        }
        else
        {
            infoUnitCanvas.SetActive(true);

            if (buildMenuCanvas != null)
                buildMenuCanvas.SetActive(true);
        }
           
        activeCanvasInfo = stateCanvas;
    }

    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!isOwned) return;

        AuthorityOnUnitDespawned?.Invoke(this);
    }

    public void Select()
    {
        //if (!hasAuthority) return;
        onSelected?.Invoke();
    }

    public void Deselect()
    {
      //  if (!hasAuthority) return;
        onDeselected?.Invoke(); 
    }
    #endregion
}
