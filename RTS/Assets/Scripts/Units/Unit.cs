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
    [SerializeField] private GameObject infoUnitCanvas = null;
    [SerializeField] private GameObject buildMenuCanvas = null;

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;

    public static event Action<GameObject> OnUnitStart;

    private bool activeCanvasInfo;

    private void Start()
    {

    }

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
      //  OnUnitStart.Invoke(this.gameObject);
        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!hasAuthority) return;

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
