using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//[Serializable]
public class PlayerName : MonoBehaviour
{
    public string playerName;
    private string playerNamePath = "nameOption.txt";
    
    private void Start()
    {
        if (!File.Exists(playerNamePath))
        {

            //nameObj.playerName = "DefaultName";

            //string nameText = JsonUtility.ToJson(nameObj);
            string nameText = "DefaultName";

            File.WriteAllText(playerNamePath, nameText);
        }
       
        playerName = File.ReadAllText(playerNamePath);
    }
}
