using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text goldResourcesText = null;
    [SerializeField] private TMP_Text woodResourcesText = null;

    private RTSPlayer player;

    private void Start()
    {
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        ClientHandleGoldResourcesUpdated(player.GetGoldResources());
        ClientHandleWoodResourcesUpdated(player.GetWoodResources());
        player.ClientOnResourcesGoldUpdated += ClientHandleGoldResourcesUpdated;
        player.ClientOnResourcesWoodUpdated += ClientHandleWoodResourcesUpdated;
    }


    private void OnDestroy()
    {
        player.ClientOnResourcesGoldUpdated -= ClientHandleGoldResourcesUpdated;
        player.ClientOnResourcesWoodUpdated -= ClientHandleWoodResourcesUpdated;
    }

    private void ClientHandleGoldResourcesUpdated(int resources)
    {
        goldResourcesText.text = $"Gold: {resources}";
    }
    private void ClientHandleWoodResourcesUpdated(int resources)
    {
        woodResourcesText.text = $"Wood: {resources}";
    }
}
