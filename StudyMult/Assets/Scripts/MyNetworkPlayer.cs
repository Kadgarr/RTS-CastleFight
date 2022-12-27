using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SerializeField] private TMP_Text displayNameText = null;
    [SerializeField] private Renderer displayColorRenderer = null;

    [SyncVar(hook = nameof(HandleDisplayPlayerNameUpdated))]
    [SerializeField]
    private string displayName = "Missing Name";

    [SyncVar(hook=nameof(HandleDisplayColorUpdated))]
    [SerializeField]
    private Color displayColor = Color.black;



    #region Server
    [Server]
    public void SetDisplayName(string newDisplayName)
    {
        if (newDisplayName.Length <= 10)
        {
            bool check = true;
            for (int i = 0; i < newDisplayName.Length; i++)
            {
                if (newDisplayName[i] == '?' || newDisplayName[i] == '/' || newDisplayName[i] == '!' || newDisplayName[i] == ' ')
                {
                    check = false;
                    break;
                }
            }

            if (check)
            {
                if (!newDisplayName.Contains("dick"))
                {
                    displayNameText.text = newDisplayName;
                }
                else
                    Debug.Log("Player name contains blacklist word!");
                
            }
            else
                Debug.Log("Player name contains special characters!");
        }
        else
            Debug.Log("Player name is too long!");
       // RpcLogNewName(displayName);
    }

    [Server]
    public void SetDisplayColor(Color newDisplayColor)
    {
        displayColor = newDisplayColor;
    }

    [Command]
    private void CmdSetDisplayName(string newDisplayName)
    {
        SetDisplayName(newDisplayName);
    }
    #endregion


    #region Clients
    private void HandleDisplayColorUpdated (Color odlColor, Color newColor)
    {
        //displayColorRenderer.material.color = newColor;
        displayColorRenderer.material.SetColor("_Color", newColor);
    }

    private void HandleDisplayPlayerNameUpdated(string oldName, string newName)
    {
        displayNameText.SetText(newName);
    }

    [ContextMenu("SetMyName")]
    private void SetMyName()
    {
        CmdSetDisplayName("My New Name");
    }

    [ClientRpc]
    private void RpcLogNewName(string newDisplayName)
    {
        Debug.Log(newDisplayName);
    }
    #endregion
}
