using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashDamage : NetworkBehaviour
{
    [SerializeField] private float splashDamage;

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Health>(out Health health) && other.TryGetComponent<Unit>(out Unit unit))
        {
            health.DealDamage(splashDamage);
            Debug.LogError($"SplashDamage:{splashDamage}");
        }
    }
}
