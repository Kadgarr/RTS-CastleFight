using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Targeter : NetworkBehaviour
{
    [SerializeField] private Targetable target;

    public Targetable GetTarget()
    {
        return target;
    }

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
        ClearTarget();
    }

    [Command]
    public void CmdSetTarget(GameObject targetGameobject)
    {
        if(!targetGameobject.TryGetComponent<Targetable>(out Targetable newTarget)) { return; }

        this.target = newTarget;

    }
    public void AttackUnit(GameObject targetGameobject)
    {
        if (!targetGameobject.TryGetComponent<Targetable>(out Targetable newTarget)) { return; }

        this.target = newTarget;

    }
    [Server]
    public void ClearTarget()
    {
        target = null;
    }
   
}
