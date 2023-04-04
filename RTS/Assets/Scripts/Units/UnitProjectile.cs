using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class UnitProjectile : NetworkBehaviour
{
    public enum TypeOfDamage
    {
        Normal = 0,
        Magic = 1,
        Pierce = 2,
        Chaos = 3,
        Siege = 4,
        Hero = 5
    }

    [SerializeField] private Rigidbody rbody=null;

    [Header("Damage Settings - Урон")]
    [SerializeField] private TypeOfDamage typeOfDamage = new TypeOfDamage();
    [Space(10)]
    [SerializeField] private int damageToDealMin = 20;
    [SerializeField] private int damageToDealMax = 30;
    [Space(10)]
    [SerializeField] private int criticalDamadeChance = 0;
    [SerializeField] private float criticalDamageModificator = 0f;
    [Space(10)]
    [SerializeField] private float destroyAfterSeconds = 1f;
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
           float damage = Random.Range(damageToDealMin, damageToDealMax);

           float modificator = matrixOfDamage[(int)typeOfDamage, (int)health.GetTypeOfArmor()];

           float summaryDamageToDeal = modificator * (damage - (damage - ((((100f - (health.GetLevelOfArmor() * 4f + 3f)) * damage) / 100f))));


            //проверяем на шанс критического урона
           if (criticalDamadeChance > 0)
           {
                int chance = Random.Range(0, 100);

                if(chance >0 && chance<= criticalDamadeChance)
                {
                    summaryDamageToDeal = summaryDamageToDeal * criticalDamageModificator;
                }
                    
           }
            //проверяем на шанс промаха
            if (health.GetChanceOfMiss() > 0)
           {
                int chance = Random.Range(0, 100);

                if(chance>0 && chance <= health.GetChanceOfMiss())
                {
                    DestroySelf();
                    
                }
                    
           }
            health.DealDamage(summaryDamageToDeal);
           
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

    public int GetCriticalDamage()
    {
        return criticalDamadeChance;
    }
}
