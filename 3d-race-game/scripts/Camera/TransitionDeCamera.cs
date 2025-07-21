using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionDeCamera : MonoBehaviour
{
    public GameObject[] cameras;
    /*
    cameras[0] = Main Camera
    cameras[1] = cameraArriere
    cameras[2] = cameraFrontale
    */

    public int cameraActive = 0;

    void Update()
    {
        ControleurDesCameras();
    }

    /*
    -----------------------------
    20.03.2025 

    Egemen YAPSIK

    Version 1.0:
    
    1) ControleurDesCameras : Cette fonction détecte l'appui sur la touche Caps Lock et change la caméra active en désactivant toutes les autres. Elle permet de passer d'une caméra à l'autre dans un ordre cyclique.
    2) EteignezLesCameras : Cette fonction désactive toutes les caméras de la liste pour s'assurer qu'une seule caméra sera activée à la fois.

    ------------------------------
    */

    void ControleurDesCameras() {
        if (Input.GetKeyDown(KeyCode.CapsLock)) {
            EteignezLesCameras();
            cameraActive++;

            if (cameraActive == cameras.Length) {
                cameraActive = 0;
            }

            cameras[cameraActive].SetActive(true);
        }
    }

    public void EteignezLesCameras() {
        foreach (var cam in cameras) {
            cam.SetActive(false);
        }
    }

    /*
    ----------------------------
    23.03.2025 
    
    Egemen YAPSIK

    Version 1.0

    1) AjusterLaCamera: Active la caméra souhaitée et désactive les autres (Je l'utilise dans CarController.cs)

    
    -----------------------------
    */
    public void AjusterLaCamera(int index) {
        EteignezLesCameras();
        cameras[index].SetActive(true);
    }
}
