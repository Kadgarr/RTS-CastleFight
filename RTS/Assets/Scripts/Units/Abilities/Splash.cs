using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Splash: NetworkBehaviour
{
    [SerializeField] private UnityEvent ActiveSplash = null;

    public override void OnStartServer()
    {
        this.gameObject.GetComponent<UnitProjectile>().hit += HandleHit;
    }

    public override void OnStopServer()
    {
        this.gameObject.GetComponent<UnitProjectile>().hit -= HandleHit;
    }

    private void HandleHit()
    {
        ActiveSplash?.Invoke(); 
    }
}
