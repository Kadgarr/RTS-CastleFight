using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defend : NetworkBehaviour
{
    [SerializeField] private UnitFiring unitFiring = null;
    [SerializeField] private UnitProjectile projectile = null;
    [SerializeField] private Health health = null;

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<UnitProjectile>(out UnitProjectile outProjectile))
        {
            if (outProjectile.GetTypeOfDamage().ToString() == "Magic")
            {
                
            }
        }
    }
}
