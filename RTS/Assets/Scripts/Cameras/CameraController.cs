using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour
{

    [SerializeField] private Transform playerCameraTransform = null;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float screenBorderThickness = 10f;
    [SerializeField] private Vector2 screenXLimits = Vector2.zero;
    [SerializeField] private Vector2 screenZLimits = Vector2.zero;

    private Controls controls;
    private Vector2 previousInput;

    public override void OnStartAuthority()
    {
        playerCameraTransform.gameObject.SetActive(true);
        
        controls = new Controls();

        controls.Player.MoveCamera.performed += SetPreviousInput;
        controls.Player.MoveCamera.canceled += SetPreviousInput;
        RTSPlayer.ClientOnTeamInfoUpdated += HandleTeamNumberUpdate;
        controls.Enable();
    }
    public override void OnStopAuthority()
    {
        RTSPlayer.ClientOnTeamInfoUpdated -= HandleTeamNumberUpdate;
        base.OnStopAuthority();
    }

    private void HandleTeamNumberUpdate()
    {
        RTSPlayer player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        
        if (player.GetTeamNumber() == 1)
        {
            playerCameraTransform.transform.position =
               new Vector3(-110f, playerCameraTransform.position.y, playerCameraTransform.position.z);
            Debug.LogError($"Number: {player.GetTeamNumber()}");
        }
        else
        {
            playerCameraTransform.transform.position =
               new Vector3(98f, playerCameraTransform.position.y, playerCameraTransform.position.z);
            Debug.LogError($"Number: {player.GetTeamNumber()}");
        }
           

    }


    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority || !Application.isFocused) return;

        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        Vector3 pos = playerCameraTransform.position;

        //если не клавиатура
        if (previousInput == Vector2.zero)
        {
            Vector3 cursorMovement = Vector3.zero;

            Vector2 cursorPosition = Mouse.current.position.ReadValue();

            if (cursorPosition.y >= Screen.height - screenBorderThickness)
            {
                cursorMovement.z += 1;
            }
            else if(cursorPosition.y <= screenBorderThickness)
            {
                cursorMovement.z -= 1;
            }

            if (cursorPosition.x >= Screen.width - screenBorderThickness)
            {
                cursorMovement.x += 1;
            }
            else if (cursorPosition.x <= screenBorderThickness)
            {
                cursorMovement.x -= 1;
            }

            pos += cursorMovement.normalized * speed * Time.deltaTime;
        }
        else
        {
            pos += new Vector3(previousInput.x, 0f, previousInput.y) * speed * Time.deltaTime;
        }

        pos.x = Mathf.Clamp(pos.x, screenXLimits.x, screenXLimits.y);
        pos.z = Mathf.Clamp(pos.z, screenZLimits.x, screenZLimits.y);

        playerCameraTransform.position = pos;
    }

    private void SetPreviousInput(InputAction.CallbackContext ctx)
    {
        previousInput = ctx.ReadValue<Vector2>();
    }

   

}
