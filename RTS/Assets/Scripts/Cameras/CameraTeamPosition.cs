using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTeamPosition : MonoBehaviour
{

    private RTSPlayer player;
    void Start()
    {
        RTSPlayer.ClientOnTeamInfoUpdated += HandleTeamNumberUpdate;
    }

    private void OnDestroy()
    {
        RTSPlayer.ClientOnTeamInfoUpdated -= HandleTeamNumberUpdate;
    }

    private void HandleTeamNumberUpdate()
    {
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        if (player.GetTeamNumber() == 1)
        {

            this.transform.position = new Vector3
             (-110f, player.GetCameraTransform().position.y, player.GetCameraTransform().position.z);


        }
        else if (player.GetTeamNumber() == 2)
        {

            this.transform.position = (new Vector3
                (98f, player.GetCameraTransform().position.y, player.GetCameraTransform().position.z));

        }
    }
}
