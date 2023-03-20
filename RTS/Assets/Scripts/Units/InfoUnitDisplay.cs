using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoUnitDisplay : MonoBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private UnitFiring unitFiring = null;

    [SerializeField] private TMP_Text healthField = null;
    [SerializeField] private TMP_Text fullHealthField = null;
    [SerializeField] private TMP_Text damageField = null;
    [SerializeField] private TMP_Text attackSpeedField = null;

    private GameObject projectile;


    private void Start()
    {
        projectile = unitFiring.GetProjectilePrefab();

        damageField.text = projectile.GetComponent<UnitProjectile>().GetDamageToDealMin().ToString();

        attackSpeedField.text = unitFiring.GetFireRate().ToString();
    }


    void Update()
    {
        healthField.text = health.GetCurrentHealth().ToString();
        fullHealthField.text = health.GetHealth().ToString();
    }
}