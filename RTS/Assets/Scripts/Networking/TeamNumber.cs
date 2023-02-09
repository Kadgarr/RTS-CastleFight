using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamNumber : MonoBehaviour
{
    [SerializeField] private int teamNumber;

    private RTSPlayer player;

    private void Start()
    {
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        teamNumber = player.GetTeamNumber();
    }

    public int GetTeamNumber()
    {
        return teamNumber;
    }
}
