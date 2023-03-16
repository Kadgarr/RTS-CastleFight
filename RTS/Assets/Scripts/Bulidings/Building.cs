using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Building : NetworkBehaviour
{
    [SerializeField] private GameObject buildingPreview = null;
    [SerializeField] private GameObject infoBuildingCanvas = null;
    [SerializeField] private Sprite icon = null;
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;
    [SerializeField] private int id = -1;
    [SerializeField] private int price = 100;

    [SerializeField] List<GameObject> nextUpgradeBuildings = null;

    private bool activeCanvasInfo;

    public static event Action<Building> ServerOnBuildingSpawned;
    public static event Action<Building> ServerOnBuildingDespawned;

    public static event Action<Building> AuthorityOnBuildingSpawned;
    public static event Action<Building> AuthorityOnBuildingDespawned;

    public GameObject GetBuildingPreview()
    {
        return buildingPreview;
    }

   
    public bool GetActiveCanvasInfo()
    {
        return activeCanvasInfo;
    }

    public void SetActiveCanvasInfo(bool stateCanvas)
    {
        if(!stateCanvas)
            infoBuildingCanvas.SetActive(false);
        else
            infoBuildingCanvas.SetActive(true);

        activeCanvasInfo =stateCanvas;
    }

    public Sprite GetIcon()
    {
        return icon;
    }

    public int GetId()
    {
        return id;
    }

    public int GetPrice()
    {
        return price;
    }

    #region Server
    public override void OnStartServer()
    {
        ServerOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnBuildingDespawned?.Invoke(this);
    }
    #endregion

    #region Client

    [Command]
    public void UpgrageBuilding(int buildingnNumber)
    {   
        GameObject buildingInstance = null;

        for (int i= 0; i < nextUpgradeBuildings.Count; i++)
        {
            if (i == buildingnNumber)
            {
                buildingInstance =
                   Instantiate(nextUpgradeBuildings[i], this.gameObject.transform.position,
                   this.gameObject.transform.rotation);
                break;
            }
                
        }
        
        NetworkServer.Spawn(buildingInstance, connectionToClient);

        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

        player.SetResources(player.GetResources() - buildingInstance.GetComponent<Building>().GetPrice());

        Destroy(this.gameObject);
    }


    public override void OnStartAuthority()
    {
        if (!hasAuthority) return;
        AuthorityOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!hasAuthority) return;

        AuthorityOnBuildingDespawned?.Invoke(this);
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
