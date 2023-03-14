using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamFogOfWar : MonoBehaviour
{
    [SerializeField] GameObject fogOfWarLeft = null;
    [SerializeField] GameObject fogOfWarRight = null;

    int teamNumber;

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

}
