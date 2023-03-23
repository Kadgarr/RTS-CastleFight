using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TypeOfArmorDisplay : MonoBehaviour
{
    [SerializeField] private GameObject infoArmorDisplay = null;
    [SerializeField] private Health health = null;
    [SerializeField] private List<TMP_Text> listofTypeDamageFields= null;
    [SerializeField] private TMP_Text fieldTypeOfArmor = null;

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
        fieldTypeOfArmor.text = fieldTypeOfArmor.text + $" {health.GetTypeOfArmor().ToString()}" ;

        int numArmor = (int)health.GetTypeOfArmor();

        for(int i = 0; i < listofTypeDamageFields.Count; i++)
        {
            listofTypeDamageFields[i].text = listofTypeDamageFields[i].text.ToString() + $" {matrixOfDamage[numArmor,i]*100}%";

            if (matrixOfDamage[numArmor, i] > 1.05)
                listofTypeDamageFields[i].color = Color.red;
            else
                if(matrixOfDamage[numArmor, i] == 1)
                listofTypeDamageFields[i].color = Color.yellow;
            else
                if (matrixOfDamage[numArmor, i] < 1)
                listofTypeDamageFields[i].color = Color.green;
        }
    }

    private void OnMouseEnter()
    {
        Debug.Log("CHECK IN);");
        infoArmorDisplay.SetActive(true);
    }

    private void OnMouseExit()
    {
        Debug.Log("CHECK OUT);");
        infoArmorDisplay.SetActive(false);
    }
}
