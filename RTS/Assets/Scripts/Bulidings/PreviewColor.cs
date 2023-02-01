using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PreviewColor : MonoBehaviour
{
    private Color normalColor;
    private void Start()
    {
        normalColor = this.gameObject.GetComponent<Renderer>().material.color;
    }
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.layer == 0) 
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);

    }

    private void OnTriggerEnter(Collider collision)
    {
       
        if (collision.gameObject.layer == 0)
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 0) 
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", normalColor);
    }
}
