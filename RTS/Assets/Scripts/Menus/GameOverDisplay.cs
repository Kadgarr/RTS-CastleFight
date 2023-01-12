using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private GameObject gameOverDisplayParent=null;
    [SerializeField] private TMP_Text winnerNameText = null;

    private void Start()
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameover;
    }


    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameover;
    }


    public void LeaveGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();    
        }
    }


    private void ClientHandleGameover(string winner)
    {
        winnerNameText.text = $" {winner} has won!";

        gameOverDisplayParent.SetActive(true);
    }
}
