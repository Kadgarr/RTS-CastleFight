using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering.Universal;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private Transform cameraTransform=null;
    [SerializeField] private LayerMask buildingBlockLayer = new LayerMask();
    [SerializeField] private LayerMask floorMask = new LayerMask();
    [SerializeField] private Building[] buildings = new Building[0];
    [SerializeField] private float buildingRangeLimit = 5f;


    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    private int resources = 500;
    [SyncVar(hook=nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool isPartyOwner=false;
    [SyncVar(hook =nameof(ClientHandleDisplayNameUpdated))]
    private string displayName;

    [SerializeField]
    [SyncVar(hook = nameof(ClientHandleTeamNumberUpdated))]
    private int teamNumber;


    public event Action<int> ClientOnResourcesUpdated;
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
    public static event Action<bool> SpawnedBuildingPreviewUpdated;
    public static event Action ClientOnInfoUpdated;
    public static event Action ClientOnTeamInfoUpdated;

    private Color teamColor = new Color();
    private List<Unit> myUnits = new List<Unit>();
    

    private List<Building> myBuildings = new List<Building>();
    private Camera mainCamera;

    private Unit unitBuilder;
    private int idBuildng;
    [SyncVar(hook = nameof(SpawnedHandleBuildingPreviewUpdated))]
    private bool checkDistanceBuilderUnit=true;

    private Vector3 placePoint;


    private void Update()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        if (!checkDistanceBuilderUnit)
        {
            CheckDistanceToUnitBuilder(placePoint,idBuildng);
        }
    }

    public string GetDisplayName()
    {
        return displayName;
    }

    public int GetTeamNumber()
    {
        return teamNumber;
    }
    public bool GetIsPartyOwenr()
    {
        return isPartyOwner;
    }
    public Transform GetCameraTransform()
    {
        return cameraTransform;
    }

    public Color GetTeamColor()
    {
        return teamColor;
    }

    public List<Unit> GetMyUnits()
    {
        return myUnits;
    }

    public List<Building> GetMyBuildings()
    {
        return myBuildings;
    }

    public int GetResources()
    {
        return resources;
    }

    [Command]
    public void SetCheckDistanceBuilderUnit(bool check)
    {
        checkDistanceBuilderUnit=check;
        unitBuilder = null;
    }

    public void CheckDistanceToUnitBuilder(Vector3 point, int idBuilding)
    {
       // Debug.Log(unitBuilder.gameObject.transform.position+ "\n"+ point);
        if (unitBuilder.gameObject.transform.position.x == point.x
            && unitBuilder.gameObject.transform.position.z == point.z)
        {
            checkDistanceBuilderUnit = true;

            TryPlaceBuilding(idBuilding, point);

        }
        else
        {
            return;
        }
            
    }

    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
    {
        if (Physics.CheckBox(point + buildingCollider.center,
           buildingCollider.size / 2,
           Quaternion.identity,
           buildingBlockLayer))
        {
            
            return false;
        }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hitt, Mathf.Infinity, buildingBlockLayer))
        {
            if (hitt.collider.gameObject.tag == "RoadArea")
                return false;
        }
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
        {
            
            if (hit.collider.gameObject.tag == "BuildingArea" )
            {
                
                if(hit.collider.TryGetComponent<TeamNumberArea>(out TeamNumberArea teamNumberArea))
                {
                    if (teamNumberArea.GetTeamNumber() != GetTeamNumber())
                    {
                        return false;
                    }
                    else if (teamNumberArea.GetTeamNumber() == GetTeamNumber())
                    {
                        return true;
                    }
                       
                }
            }
            if (hit.collider.gameObject.tag == "WallArea")
            {
                return false;
            }

        }

       
        return true;
    }


    //foreach (Building building in myBuildings)
    //{
    //    if ((point - building.transform.position).sqrMagnitude <=
    //        buildingRangeLimit * buildingRangeLimit)
    //    {

    //        return true;
    //    }
    //}

    #region Server

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }

    [Server]
    public void SetPartyOwner(bool state)
    {
        isPartyOwner = state;
    }

    [Command]
    public void CmdSetTeamNumber(int teamNumber)
    {
        this.teamNumber=teamNumber;
    }
    [Command]
    public void CmdSetCameraTransform(Vector3 cameraPosition)
    {
        this.cameraTransform.position = cameraPosition;
    }
    [Command]
    public void CmdStartGame()
    {
        if (!isPartyOwner) return;
        ((RTSNetworkManager)NetworkManager.singleton).StartGame();

    }
    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;

        DontDestroyOnLoad(gameObject);
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
    }

    [Server]
    public void SetTeamColor(Color newTeamColor)
    {
        teamColor = newTeamColor;
    }

    [Server]
    public void SetResources(int newResources)
    {
        resources = newResources;
    }

    [Command]
    public void CmdTryPlaceBuilding(int idBuilding, Vector3 point)
    {
        Building buildingToPlace = null;

        foreach (Building building in buildings)
        {
            if(building.GetId()==idBuilding)
            {
                buildingToPlace = building;
                break;
            }
        }

        if (buildingToPlace == null) return;

        if (resources < buildingToPlace.GetPrice()) return; 

        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();

       
        if (!CanPlaceBuilding(buildingCollider,point)) return; //???????? ????? ?? ??????? ??????

        placePoint = point;

        idBuildng = idBuilding;
        

        foreach (Unit unit in myUnits)
        {
            if (unit.gameObject.layer == 12)
            {
                if (unitBuilder == null)
                    checkDistanceBuilderUnit = false;

                unit.gameObject.GetComponent<NavMeshAgent>().SetDestination(point);

                unitBuilder = unit;

                break;
            }
        }
        //???????? ?????????, ???? ?????? ?? ?????? ?????????, ?? ????? ?? ??????????

        if (!checkDistanceBuilderUnit) return;
        
        GameObject buildingInstance=
            Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);
        
        
        NetworkServer.Spawn(buildingInstance, connectionToClient);

        SetResources(resources - buildingToPlace.GetPrice());
        unitBuilder = null;
    }

    public void TryPlaceBuilding(int idBuilding, Vector3 point)
    {
        Building buildingToPlace = null;

        foreach (Building building in buildings)
        {
            if (building.GetId() == idBuilding)
            {
                buildingToPlace = building;
                break;
            }
        }


        if (buildingToPlace == null) return;

        if (resources < buildingToPlace.GetPrice()) return;

        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();

       // if (!CanPlaceBuilding(buildingCollider, point)) return;

        GameObject buildingInstance =
            Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);

        NetworkServer.Spawn(buildingInstance, connectionToClient);

        SetResources(resources - buildingToPlace.GetPrice());
        unitBuilder = null;
    }

    private void ServerHandleUnitSpawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;

        myUnits.Add(unit);
    }

    private void ServerHandleUnitDespawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;

        myUnits.Remove(unit);
    }

    private void ServerHandleBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;

        myBuildings.Add(building);
    }

    private void ServerHandleBuildingDespawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;

        myBuildings.Remove(building);
    }
    #endregion



    #region Client
     
 
    public override void OnStartAuthority()
    {
        if (NetworkServer.active)  return; 

        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
    }

    public override void OnStartClient()
    {
        if (NetworkServer.active) return;

        DontDestroyOnLoad(gameObject);
       
        ((RTSNetworkManager)NetworkManager.singleton).Players.Add(this); 

    }

    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        if (!isClientOnly || !hasAuthority) return;

        ((RTSNetworkManager)NetworkManager.singleton).Players.Remove(this);

        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }

    private void ClientHandleTeamNumberUpdated(int oldTeamNumber, int newTeamNumber)
    {
        ClientOnTeamInfoUpdated?.Invoke(); 
    }

    private void ClientHandleDisplayNameUpdated(string oldDisplayName, string newDisplayName)
    {
        ClientOnInfoUpdated?.Invoke();
    }
    private void ClientHandleResourcesUpdated(int oldResources, int newResources)
    {
        ClientOnResourcesUpdated?.Invoke(newResources);
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority) return;

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }

    private void SpawnedHandleBuildingPreviewUpdated(bool oldState, bool newState)
    {
        SpawnedBuildingPreviewUpdated.Invoke(newState);
    }

    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        myUnits.Add(unit);
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        myUnits.Remove(unit);
    }

    private void AuthorityHandleBuildingDespawned(Building building)
    {
        myBuildings.Remove(building);
    }

    private void AuthorityHandleBuildingSpawned(Building building)
    {
        myBuildings.Add(building);
    }
    #endregion
}
