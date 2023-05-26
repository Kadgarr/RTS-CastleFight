using Mirror.Examples.Tanks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InfoOfUpradeBuilding : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject infoBuildingDisplay = null;
    [SerializeField] private int numberOfUpgrade = 0;
    [SerializeField] private Building building = null;
    [SerializeField] private TMP_Text fieldOfNameUnit = null;
    [SerializeField] private TMP_Text fieldTypeOfArmor = null;
    [SerializeField] private TMP_Text fieldTypeOfDamage = null;
    [SerializeField] private TMP_Text fieldLevelOfDamage = null;
    [SerializeField] private TMP_Text fieldDescriptionAbilities = null;

    [SerializeField] private bool upgrade=false;

    private void Start()
    {
        GameObject unit = null;
        if(upgrade)
        unit = building.GetListUpgrades()[numberOfUpgrade].gameObject.GetComponent<UnitSpawner>().GetUnit();
        else
            unit = building.gameObject.GetComponent<UnitSpawner>().GetUnit();

        var health = unit.GetComponent<Health>();
        var projectile = unit.GetComponent<UnitFiring>().GetProjectilePrefab().GetComponent<UnitProjectile>();

        fieldOfNameUnit.text = unit.gameObject.name;

        fieldTypeOfArmor.text = fieldTypeOfArmor.text + $" {health.GetTypeOfArmor().ToString()} " +
            $"({health.GetLevelOfArmor()})";

        fieldLevelOfDamage.text = fieldLevelOfDamage.text + $" {projectile.GetDamageToDealMin()} - {projectile.GetDamageToDealMax()}";

        fieldTypeOfDamage.text = fieldTypeOfDamage.text + $" {projectile.GetTypeOfDamage()}";

        if (projectile.GetCriticalDamage() > 0)
            fieldDescriptionAbilities.text = fieldDescriptionAbilities.text + $" Critical damage {projectile.GetCriticalDamage()}%;";
        
        if(health.GetChanceOfMiss()>0)
            fieldDescriptionAbilities.text = fieldDescriptionAbilities.text + $" Evade {health.GetChanceOfMiss()}%";

        fieldDescriptionAbilities.text = fieldDescriptionAbilities.text + $" {unit.GetComponent<UnitFiring>().GetDescriptionOfAbilities()}";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        infoBuildingDisplay.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        infoBuildingDisplay.SetActive(false);
        
    }

    public int GetNumberOfUpdgrade()
    {
        return numberOfUpgrade;
    }
}
