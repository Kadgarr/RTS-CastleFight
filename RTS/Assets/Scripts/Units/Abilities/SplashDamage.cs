using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class SplashDamage : NetworkBehaviour
{
    [SerializeField] private float splashDamage;

    private void Start()
    {
        Debug.LogError("Start");
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        Debug.LogError($"SplashDamage 0:{splashDamage}");
        if (other.TryGetComponent<Health>(out Health health) && other.TryGetComponent<Unit>(out Unit unit))
        {
            health.DealDamage(splashDamage);
            Debug.LogError($"SplashDamage:{splashDamage}");
            Destroy(gameObject);
        }
    }
}
