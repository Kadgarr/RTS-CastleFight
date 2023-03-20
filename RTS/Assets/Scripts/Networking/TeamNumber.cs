using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamNumber : NetworkBehaviour
{
    [SerializeField] private int teamNumber;

    RTSPlayer player;

    //[ServerCallback]
    private void Update()
    {
        if(teamNumber == 0)
        {
            player = connectionToClient.identity.GetComponent<RTSPlayer>();

            teamNumber = player.GetTeamNumber();

        }
    }

    public int GetTeamNumber()
    {
        return teamNumber;
    }
}
