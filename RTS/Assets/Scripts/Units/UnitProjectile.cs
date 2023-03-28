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

    private float[,] matrixOfDamage = new float[,] { 
                                                    {1f, 1.75f, 0.7f, 0.25f, 0.6f, 0.5f, 1.05f },
                                                    { 1.75f, 0.7f, 1f, 0.25f, 0.6f, 0.4f, 1.05f },
                                                    { 0.7f, 1f, 1.75f, 0.25f, 0.6f, 0.5f, 1.05f },
                                                    { 1f, 1f, 1f, 1f, 1f, 1f, 1f },
                                                    { 0.7f, 0.7f, 0.7f, 0.2f, 0.4f, 1.6f, 1f },
                                                    { 1.1f, 1.1f, 1.1f, 0.4f, 0.4f, 0.6f, 1.1f} 
    };

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
           int damageToDeal = Random.Range(damageToDealMin, damageToDealMax);
           int modificator = (int)matrixOfDamage[(int)typeOfDamage, (int)health.GetTypeOfArmor()];

           health.DealDamage(modificator * (damageToDeal-(damageToDeal - ((((100-(health.GetLevelOfArmor()*4+3))* damageToDeal)/100)))));

           Debug.Log($"Type {typeOfDamage}; Modificator {matrixOfDamage[((int)typeOfDamage), (int)health.GetTypeOfArmor()]}");
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
        return damageToDealMax;
    }

    public TypeOfDamage GetTypeOfDamage()
    {
        return typeOfDamage;
    }
}
