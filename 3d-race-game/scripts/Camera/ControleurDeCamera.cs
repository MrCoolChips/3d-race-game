using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleurDeCamera : MonoBehaviour
{
    public Transform[] objectif;
    // objectif[0] = positionCamera
    // objectif[1] = angleCamera


    /*
    -----------------------------
    21.03.2025 

    Egemen YAPSIK

    Version 1.0:
    
    Avec cette méthode, nous pouvons détacher la caméra de l'objet enfant et l'utiliser indépendamment.
    De cette façon, la caméra ne tremble pas lorsque la voiture passe sur des bosses ou différentes élévations.
    -----------------------------
    */

    void LateUpdate()
    {
        if (objectif[0] != null) {
            transform.position = objectif[0].position;
            transform.LookAt(objectif[1].position);
        }
        
    }


}
