using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Splash: NetworkBehaviour
{

    RaycastHit hit;

    [SerializeField] public LayerMask layerMask = new LayerMask();
    [SerializeField] public float radius;
    [SerializeField] public float splashDamage;

    private List<Collider> colliderArray = new List<Collider>();
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
     
        colliderArray = Physics.OverlapSphere(this.gameObject.transform.position, radius, layerMask).ToList();
        Debug.Log("List objects");
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
            {
                if (networkIdentity.connectionToClient.identity.GetComponent<RTSPlayer>().GetTeamNumber()
                    != connectionToClient.identity.GetComponent<RTSPlayer>().GetTeamNumber())
                {
                    Debug.LogError(collider.gameObject.name);
                    if (collider.TryGetComponent<Health>(out Health health))
                    {
                        health.DealDamage(splashDamage);
                        Debug.LogError($"SplashDamage:{splashDamage}");
                    }
                }
            }
                    
        }

    }
}
