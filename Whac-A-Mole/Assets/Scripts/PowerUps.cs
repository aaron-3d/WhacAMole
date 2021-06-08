using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    int Probabilidad;

    Vector3 originalPosition;

    float timeToAppear = 5f;
    float appearTimer;
    Vector3 newPos;

    // Start is called before the first frame update
    void Start()
    {
        Probabilidad = Random.Range(0, 100);
        originalPosition = new Vector3(-1.91f, 2.49f, -2.64f);
        newPos = new Vector3(7.31f, 2.49f, -2.64f);

        if (Probabilidad <= 20) //Se genera un valor entre 1 y 100, si el numero está entre el 0 y el 20, se genera el powerUp azul(tiempo lento).
        {
            Debug.Log("Se ha activado el powerUp azul");
            appearTimer = Random.Range(2f, 50f);    //Se genera un valor aleatorio que determina en que momento aparece el powerUp.    
            originalPosition = transform.position;
        }

        if (Probabilidad >= 50)
        {
            Debug.Log("Se ha activado el powerUp amarillo"); //Se genera un valor entre 1 y 100, si el numero es superior a 50, se genera el powerUp amarillo(bomba).        
            appearTimer = Random.Range(2f, 50f);    //Se genera un valor aleatorio que determina en que momento aparece el powerUp.               
            originalPosition = transform.position;
        }

    }

    // Update is called once per frame
    void Update()
    {
        timeToAppear += Time.deltaTime;
        if (timeToAppear == appearTimer)
        {
            LeanTween.move(gameObject, newPos, 4f);  //Mueve el gameObject de forma gradual, a lo largo de 4 segundos.
        }
    }
}
