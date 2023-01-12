using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent=null;
    [SerializeField] private Targeter targeter=null;
    [SerializeField] private float chaseRange = 10f;

    //private Camera mainCamera;

    #region Server

    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [Server]
    private void ServerHandleGameOver()
    {
        agent.ResetPath();
    }

    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.GetTarget();

      
        if (target != null)
        {
            if((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
            {
                agent.SetDestination(target.transform.position);
            }
            else if (agent.hasPath)
            {
                agent.ResetPath();
            }

            return;
        }

        if (!agent.hasPath) return;

        if (agent.remainingDistance > agent.stoppingDistance) return;

        agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        targeter.ClearTarget();

        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }

        agent.SetDestination(hit.position);
    }

    #endregion

    #region Client

    //public override void OnStartAuthority()
    //{
    //    mainCamera = Camera.main;
    //}

    //[ClientCallback]
    //private void Update()
    //{
    //    if(!hasAuthority) { return; }

    //    if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }

    //    Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

    //    if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) { return; }

    //    CmdMove(hit.point);
    //}
    #endregion
}
