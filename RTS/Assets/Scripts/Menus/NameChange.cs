using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Mirror;

public class NameChange : MonoBehaviour
{
    private string path;
    private string newName;

    [SerializeField]
    private GameObject messageWindow = null;

    //private PlayerName playerName;

    [SerializeField] private TMP_InputField addressInput = null;

    private void Start()
    {
        path = "nameOption.txt";

        addressInput.text= File.ReadAllText(path);
        //playerName = new PlayerName();
    }

    public void SaveChangesName()
    {
        if (!File.Exists(path))
        {
            //PlayerName playerName = new PlayerName();

            //playerName.playerName = "DefaultName";

            //string name = JsonUtility.ToJson(playerName);
            string name = "DefalutName";
            File.WriteAllText(path, name);
        }

        //playerName.playerName = addressInput.text;

        //newName = JsonUtility.ToJson(playerName);
        newName = addressInput.text;
        File.WriteAllText(path, newName);
        messageWindow.SetActive(true);
    }
}
