using Mirror.Examples.Tanks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TypeOfDamageDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject infoDamageDisplay = null;
    [SerializeField] private UnitProjectile projectile = null;
    [SerializeField] private List<TMP_Text> listofTypeArmorFields= null;
    [SerializeField] private TMP_Text fieldTypeOfDamage = null;
    [SerializeField] private TMP_Text fieldLevelOfDamage = null;

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
        fieldLevelOfDamage.text = fieldLevelOfDamage.text + $" {projectile.GetDamageToDealMin()} - {projectile.GetDamageToDealMax()}";

        //fieldTypeOfDamage.text = fieldTypeOfDamage.text + $" {projectile.GetTypeOfDamage().ToString()}";

        //int numDamage = (int)projectile.GetTypeOfDamage();

        //for (int i = 0; i < listofTypeArmorFields.Count; i++)
        //{
        //    listofTypeArmorFields[i].text = listofTypeArmorFields[i].text.ToString() + $" {matrixOfDamage[numDamage, i] * 100}%";

        //    if (matrixOfDamage[numDamage, i] > 1.05)
        //        listofTypeArmorFields[i].color = Color.green;
        //    else
        //        if (matrixOfDamage[numDamage, i] >= 1 && matrixOfDamage[numDamage, i] <= 1.05f)
        //        listofTypeArmorFields[i].color = Color.yellow;
        //    else
        //        if (matrixOfDamage[numDamage, i] < 1)
        //        listofTypeArmorFields[i].color = Color.red;
        //}
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        infoDamageDisplay.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        infoDamageDisplay.SetActive(false);
    }
}
