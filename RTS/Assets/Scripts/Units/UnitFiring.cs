using Mirror;

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint = null;
    [SerializeField] private float fireRange = 5f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private LayerMask layerMask = new LayerMask();

    private List<GameObject> unitBases = new List<GameObject>();

    private float lastFireTime;
    private Targetable target;

    private void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            unitBases.Add(GameObject.Find($"UnitBase(Clone) {i + 1}"));
        }

    }

    [ServerCallback]
    private void Update()
    {
        target = targeter.GetTarget();

        if (target == null)
        {
            
            AutoAttack();

            TargetUnitBase();


            return;
        }

        if(!CanFireAtTarget()) return;

       
        Quaternion targetRotation = Quaternion.LookRotation(
            target.transform.position - transform.position);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,targetRotation,rotationSpeed*Time.deltaTime);

        if (Time.time > (1 / fireRate) + lastFireTime)
        {
            Quaternion projectileRotation = Quaternion.LookRotation(
                target.GetAimPoint().position - projectileSpawnPoint.position);
            
            GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoint.position,projectileRotation);

            NetworkServer.Spawn(projectileInstance, connectionToClient);

            lastFireTime = Time.time;
        }
    }



    private void AutoAttack()
    {
        if (Physics.SphereCast(projectileSpawnPoint.position, fireRange, transform.position, out RaycastHit hit, fireRange, layerMask))
        {
           
            if (hit.collider.TryGetComponent<Targetable>(out Targetable potentialTarget))
            {
                if (potentialTarget.connectionToClient == connectionToClient)
                {
                    return;  
                }
                else
                {
                    targeter.AttackUnit(hit.collider.gameObject); 
                    target = targeter.GetTarget(); 
                }
            }
        }
     
    }

    private void TargetUnitBase()
    {
        foreach (var unitBase in unitBases)
        {
            if (unitBase == null) return;

            if (unitBase.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
            {
                if (networkIdentity.connectionToClient.connectionId != connectionToClient.connectionId)
                {
                    Debug.Log($"UnitBase:{unitBase.name}");

                    target = targeter.GetTarget();

                    targeter.AttackUnit(unitBase);
                }

            }
        }

    }
    [Server]
    private bool CanFireAtTarget()
    {
        return (targeter.GetTarget().transform.position - transform.position).sqrMagnitude
            <= fireRange * fireRange;
    }


}
