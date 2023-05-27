using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoUnitDisplay : MonoBehaviour
{
    
    [SerializeField] private UnitFiring unitFiring = null;
    [Header("Health fields")]
    [SerializeField] private Health health = null;
    [SerializeField] private TMP_Text healthField = null;
    [SerializeField] private TMP_Text fullHealthField = null;
    [SerializeField] private TMP_Text armorTypeField = null;
    [Header("Damage fields")]
    [SerializeField] private TMP_Text damageTypeField = null;
    [SerializeField] private TMP_Text damageMinField = null;
    [SerializeField] private TMP_Text damageMaxField = null;

    [SerializeField] private TMP_Text attackSpeedField = null;

    [Header("Mana fields")]
    [SerializeField] private TMP_Text currentManaField = null;
    [SerializeField] private TMP_Text fullManaField = null;

    private GameObject projectile;


    private void Start()
    {
        projectile = unitFiring.GetProjectilePrefab();

        damageTypeField.text = projectile.GetComponent<UnitProjectile>().GetTypeOfDamage().ToString();
        damageMinField.text = projectile.GetComponent<UnitProjectile>().GetDamageToDealMin().ToString();
        damageMaxField.text = projectile.GetComponent<UnitProjectile>().GetDamageToDealMax().ToString();

        attackSpeedField.text = unitFiring.GetFireRate().ToString();

        armorTypeField.text = health.GetTypeOfArmor().ToString();

        fullHealthField.text = health.GetHealth().ToString();

        if (unitFiring.GetFullMana() != 0)
            fullManaField.text = unitFiring.GetFullMana().ToString();
    }


    void Update()
    {
        healthField.text = health.GetCurrentHealth().ToString();

        currentManaField.text = unitFiring.GetCurrenStateOfMana().ToString();
    }
}