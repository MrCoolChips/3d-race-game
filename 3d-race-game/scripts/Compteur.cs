using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityStandardAssets.Vehicles.Car;

public class Compteur : MonoBehaviour
{
    public TextMeshProUGUI compteur;
    public GameObject cmpt;
    float secondes = 3f;
    public AudioSource son;
    List<Voitures> vehicules;
    bool course = false;
    void Start()
    {
        compteur.text = secondes.ToString();
        vehicules = gameObject.GetComponent<TimerAndPosition>().cars;
        StartCoroutine(CompteARebours());
        son.Play();
    }

    IEnumerator CompteARebours() {
        while (!course) {
            yield return new WaitForSeconds(1f);
            secondes--;
            compteur.text = secondes.ToString();
            if (secondes == 0) {
                foreach (var v in vehicules) {
                    if (v.voiture.gameObject.name == "YOU") {
                        v.voiture.GetComponentInParent<CarUserControl>().enabled = true;
                    }else {
                        v.voiture.GetComponentInParent<CarComputerControl>().enabled = true;
                    }
                }
                course = true;
                compteur.enabled = false;
                cmpt.SetActive(false);
            }
        }
    }
}
