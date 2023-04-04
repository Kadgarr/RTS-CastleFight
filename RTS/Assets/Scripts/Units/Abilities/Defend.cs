using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defend : NetworkBehaviour
{
    [SerializeField] private Health health = null;

    private UnitProjectile projectile = null;
    private float fireRange;


    public override void OnStartServer()
    {
        health.GetDamage += HandleGetDamage;
        UnitFiring.SpawnProjectile += HandleSpawnProjectile;
    }

    public override void OnStopServer()
    {
        health.GetDamage -= HandleGetDamage;
        UnitFiring.SpawnProjectile -= HandleSpawnProjectile;
    }

    private void HandleGetDamage(float damage)
    {
        if (fireRange > 3f)
        {
            damage = damage * 0.4f;
            health.SetDamage(damage);
        }
        else
            if (projectile.GetTypeOfDamage().ToString() == "Magic")
        {
            damage = damage * 0.5f;
            health.SetDamage(damage);
        }
        else 
            if(projectile.GetTypeOfDamage().ToString() == "Magic" && fireRange > 3f)
        {
            damage = damage * 0.5f;
            health.SetDamage(damage);
        }
            
        
    }

    private void HandleSpawnProjectile(GameObject projectileObj, float fireRange,int instanceId)
    {
        if (instanceId != gameObject.GetInstanceID())
            return;

        Debug.Log($"Instance ID: переданный {instanceId}; текущий {gameObject.GetInstanceID()};");
        projectile = projectileObj.GetComponent<UnitProjectile>();
        this.fireRange = fireRange;
    }

}
