using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoBuildingDisplay : MonoBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private UnitSpawner spawner = null;

    [SerializeField] private TMP_Text healthField=null;
    [SerializeField] private TMP_Text spawnDurationField = null;



    void Update()
    {
        healthField.text = health.GetHealth().ToString();
       
        if (spawner == null) return;

        spawnDurationField.text = spawner.GetUnitSpawnDuration().ToString();
    }
}
