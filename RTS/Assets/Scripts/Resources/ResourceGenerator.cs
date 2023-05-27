using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : NetworkBehaviour
{
    //[SerializeField] private Health health = null;
    [SerializeField] private int resourcesPerInterval = 5;
    [SerializeField] private float interval = 10f;

    private float timer;
    private RTSPlayer player;

    public override void OnStartServer()
    {
        timer = interval;
        player = connectionToClient.identity.GetComponent<RTSPlayer>();

        //health.ServerOnDie += ServerHandleDie;
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        
    }

    public override void OnStopServer()
    {
        //health.ServerOnDie -= ServerHandleDie;
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
       
    }

    public override void OnStartAuthority()
    {
        Building.AuthorityOnBuildingSpawned += HandleOnBuildingSpawned;
    }

    public override void OnStopAuthority()
    {
        Building.AuthorityOnBuildingSpawned -= HandleOnBuildingSpawned;

    }
    [ServerCallback]
    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            player.SetResourcesGold(player.GetGoldResources() + resourcesPerInterval);

            timer += interval;
        }
    }

    [Command]
    private void HandleOnBuildingSpawned(Building building)
    {
        if(building.GetWood()==0)
        player.SetResourcesWood(player.GetWoodResources()+building.GetPrice());
    }

    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    private void ServerHandleGameOver()
    {
        enabled = false;
    }

}
