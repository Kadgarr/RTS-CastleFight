using Mirror;
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


    [Command]
    public void CmdSetTarget(GameObject targetGameobject)
    {
        if(!targetGameobject.TryGetComponent<Targetable>(out Targetable newTarget)) { return; }

        this.target = newTarget;
    }

    [Server]
    public void ClearTarget()
    {
        target = null;
    }
   
}
