using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOfDamage
{
    Normal = 0,
    Magic = 1,
    Pierce = 2,
    Chaos = 3,
    Siege = 4,
    Hero = 5
}

public class UnitProjectile : NetworkBehaviour
{

    [SerializeField] private Rigidbody rbody=null;

    [Header("Damage Settings - Урон")]
    [SerializeField] private TypeOfDamage typeOfDamage = new TypeOfDamage();
    [Space(10)]
    [SerializeField] private int damageToDealMin = 20;
    [SerializeField] private int damageToDealMax = 30;
    [Space(10)]
    [SerializeField] private float destroyAfterSeconds = 5f;
    [SerializeField] private float launchForce = 10f;


    private void Start()
    {
        rbody.velocity = transform.forward * launchForce;
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfterSeconds);
    }


    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        if (other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
        {
           if (networkIdentity.connectionToClient == connectionToClient) return;
        }

        if (other.TryGetComponent<Health>(out Health health))
        {
           health.DealDamage(Random.Range(damageToDealMin,damageToDealMax));
        }

        DestroySelf();
    }


    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    public int GetDamageToDealMin()
    {
        return damageToDealMin;    
    }

    public int GetDamageToDealMax()
    {
        return damageToDealMin;
    }
}
