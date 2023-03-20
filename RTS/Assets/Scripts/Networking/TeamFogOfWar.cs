using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamFogOfWar : MonoBehaviour
{
    [SerializeField] GameObject fogOfWarLeft = null;
    [SerializeField] GameObject fogOfWarRight = null;

    int teamNumber;
    bool check=false;

    List<GameObject> units = new List<GameObject>();

    RTSPlayer player;

    private void Start()
    {
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

        teamNumber = player.GetTeamNumber();

        if (teamNumber == 1)
            fogOfWarRight.SetActive(true);
        else
            fogOfWarLeft.SetActive(true);
    }

    [ServerCallback]
    private void Update()
    {
        //if (check)
        //{
        //    if(teamNumber!=player.)
        //    check = false;
        //}
    }
    private void UnitSpawnedHandle(GameObject unitObj)
    {
        //check = true;
        //units.Add(unitObj);
        //Debug.Log("Unit " + 
        //    unitObj.GetComponent<Unit>().connectionToClient.identity.GetComponent<RTSPlayer>().GetTeamNumber());

        //if (unitObj.GetComponent<Unit>().connectionToClient.identity.GetComponent<RTSPlayer>().GetTeamNumber() == teamNumber)
        //    unitObj.GetComponent<VisionComponent>().SetVisionRange(7f);

    }
}
