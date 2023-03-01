using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField] private UnitSelectionHandler unitSelectionHandler = null;
    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Camera mainCamera;


    private void Start()
    {
        mainCamera = Camera.main;
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }
     
    private void Update()
    {
        if(!Mouse.current.rightButton.wasPressedThisFrame) return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        //if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {

            if (hit.collider.gameObject.tag == "BuildingArea" && 
                hit.collider.TryGetComponent<TeamNumberArea>(out TeamNumberArea teamNumberArea))
            {
                if (teamNumberArea.GetTeamNumber() !=
                    NetworkClient.connection.identity.GetComponent<RTSPlayer>().GetTeamNumber())
                    return;

                if (hit.collider.TryGetComponent<Targetable>(out Targetable target))
                {
                    if (target.hasAuthority)
                    {
                        TryMove(hit.point);
                        return;
                    }

                    TryTarget(target);
                    return;
                }

                TryMove(hit.point);
            }
            else
            if (hit.collider.gameObject.tag == "WallArea")
            {
                return;
            }
        }

    }

   
    private void TryMove(Vector3 point)
    {
        foreach(Unit unit in unitSelectionHandler.SelectedUnits)
        {
            if(unit.gameObject.layer==12) //указываем, что нужно двигатьс€ только билдеру
              unit.GetUnitMovement().CmdMove(point);
        }
    }

    private void TryTarget(Targetable target)
    {
        foreach (Unit unit in unitSelectionHandler.SelectedUnits)
        {
            if (unit.gameObject.layer == 12)
                unit.GetTargeter().CmdSetTarget(target.gameObject);
        }
    }

    private void ClientHandleGameOver(string winnerName)
    {
        enabled = false;
    }

}
