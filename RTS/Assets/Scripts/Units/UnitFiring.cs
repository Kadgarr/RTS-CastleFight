using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnitProjectile;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint = null;
    [SerializeField] private float fireRange = 5f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private LayerMask layerMask = new LayerMask();

    [TextAreaAttribute]
    [SerializeField] private string descriptionOfAbilites = null;

    private List<GameObject> unitBases = new List<GameObject>();

    public static event Action<GameObject,float,int> SpawnProjectile;

    private float lastFireTime;
    private Targetable target;
    private bool activeUnitBase = false;
    

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
            TargetUnitBase();

            return;
        }

        if (!CanFireAtTarget()) return;


        Quaternion targetRotation = Quaternion.LookRotation(
            target.transform.position - transform.position);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (Time.time > /*(1f / fireRate) */fireRate + lastFireTime)
        {
            Quaternion projectileRotation = Quaternion.LookRotation(
                target.GetAimPoint().position - projectileSpawnPoint.position);

            GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation);
            
            SpawnProjectile?.Invoke(projectilePrefab,fireRange, target.gameObject.GetInstanceID());

            NetworkServer.Spawn(projectileInstance, connectionToClient); 

            lastFireTime = Time.time;
        }
    }


    [ServerCallback]
    private void OnTriggerStay(Collider other)
    {

        if (other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
        {
            if (networkIdentity.connectionToClient.identity.GetComponent<RTSPlayer>().GetTeamNumber() 
                == connectionToClient.identity.GetComponent<RTSPlayer>().GetTeamNumber())
            {
                return; 
            }
        }

        if(other.TryGetComponent<Unit>( out Unit unit))
        {
            if ((unit.GetIsFlyingOnly() && this.gameObject.GetComponent<Unit>().GetIsGroundOnly())
                || (unit.GetIsGroundOnly() && this.gameObject.GetComponent<Unit>().GetIsFlyingOnly()))
                return;
        }
       
        if (target != null && !activeUnitBase) return;

        targeter.SetTarget(other.gameObject);

        target = targeter.GetTarget();

        activeUnitBase = false;

    }

    private void TargetUnitBase()
    {
        foreach (var unitBase in unitBases)
        {
            if (unitBase == null) return;

            if (unitBase.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
            {
                 
                if (networkIdentity.connectionToClient.identity.GetComponent<RTSPlayer>().GetTeamNumber() 
                    != connectionToClient.identity.GetComponent<RTSPlayer>().GetTeamNumber())
                {
                    gameObject.GetComponent<NavMeshAgent>().SetDestination(unitBase.transform.position);
                    activeUnitBase = true;
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

    public GameObject GetProjectilePrefab()
    {
        return projectilePrefab;
    }
    public float GetFireRate()
    {
        return fireRate;
    }

    public string GetDescriptionOfAbilities()
    {
        return descriptionOfAbilites;
    }

    public float GetFireRange()
    {
        return fireRange;
    }
}
