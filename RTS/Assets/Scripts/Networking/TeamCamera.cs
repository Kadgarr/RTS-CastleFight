using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamCamera : NetworkBehaviour
{
    [SerializeField] private int teamNumber;

    RTSPlayer player;
    private void Start()
    {
        if (player == null)
            player = connectionToClient.identity.GetComponent<RTSPlayer>();

        teamNumber = player.GetTeamNumber();

        if (teamNumber == 1)
            this.gameObject.transform.position = new Vector3(-110,30,-5);


    }
}
