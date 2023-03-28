using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TypeOfArmorDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject infoArmorDisplay = null;
    [SerializeField] private Health health = null;
    [SerializeField] private List<TMP_Text> listofTypeDamageFields= null;
    [SerializeField] private TMP_Text fieldTypeOfArmor = null;
    [SerializeField] private TMP_Text fieldLevelOfArmor = null;

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
        //fieldLevelOfArmor.text = fieldLevelOfArmor.text + $" {health.GetLevelOfArmor().ToString()}";

        fieldTypeOfArmor.text = fieldTypeOfArmor.text + $" {health.GetTypeOfArmor().ToString()}" ;

        int numArmor = (int)health.GetTypeOfArmor();

        for(int i = 0; i < listofTypeDamageFields.Count; i++)
        {
            listofTypeDamageFields[i].text = listofTypeDamageFields[i].text.ToString() + $" {matrixOfDamage[i, numArmor] *100}%";

            if (matrixOfDamage[i, numArmor] > 1.05)
                listofTypeDamageFields[i].color = Color.red;
            else
                if(matrixOfDamage[i, numArmor] == 1)
                listofTypeDamageFields[i].color = Color.yellow;
            else
                if (matrixOfDamage[i, numArmor] < 1)
                listofTypeDamageFields[i].color = Color.green;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        infoArmorDisplay.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        infoArmorDisplay.SetActive(false);
    }
}
