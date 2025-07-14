// Prenom : Egemen 
// Nom: YAPSIK 
// Date : 04/04/2025
// Objectif du script : Un menu qui aide à régler les paramètres de la course. Avec ce menu, vous pouvez choisir combien de personnes joueront, 
//quel type de véhicule sera utilisé, combien de tours (LAP) il y aura, et enfin, quelle carte sera utilisée. 
//Les informations saisies ici sont envoyées au GameController via la méthode DemarrerLeJeu() de la classe SelectionDuVoiture.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;

public class MenuParametresDesCours : MonoBehaviour
{   
    public AudioSource[] button;
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private TextMeshProUGUI lapText;
    [SerializeField] private TextMeshProUGUI carTypeText;
    public int nombreDeJoueur = 2;
    public int nombreDeLAP = 1;
    public String[] carType = {"RANDOM", "SAME", "ALL"};
    public int indexCarType = 0;
    public GameObject[] cartes;
    public int carteIndex = 0;
    public GameObject menuSuivant;
    public GameObject menuPrecedent;
    

    void Start() {
        playerText.text = "2";
        lapText.text = "1";
        carTypeText.text = carType[indexCarType];
    }

    public void FlecheDroiteDePlayer() {
        //On décide combien de personnes joueront
        button[0].Play();
        //Étant donné que le type ALL implique qu'une voiture de chaque type participe à la course et que nous avons 6 types de véhicules, il est fixé à 6
        if (carType[indexCarType].CompareTo("ALL") != 0) {
            if (nombreDeJoueur < 8) {
                nombreDeJoueur++;
            } else {
                nombreDeJoueur = 2;
            }   
            playerText.text = nombreDeJoueur.ToString();
        }
    }

    public void FlecheGaucheDePlayer() {
        //On décide combien de personnes joueront
        button[0].Play();
        //Étant donné que le type ALL implique qu'une voiture de chaque type participe à la course et que nous avons 6 types de véhicules, il est fixé à 6
        if (carType[indexCarType].CompareTo("ALL") != 0) {
            if (nombreDeJoueur > 2) {
                nombreDeJoueur--;
            } else {
                nombreDeJoueur = 8;
            }   
            playerText.text = nombreDeJoueur.ToString();
        }
    }

    public void FlecheGaucheDeLap() {
        //On choisit combien de tours il y aura dans la course
        button[0].Play();
        if (nombreDeLAP > 1) {
            nombreDeLAP--;
        } else {
            nombreDeLAP = 5;
        }   
        lapText.text = nombreDeLAP.ToString();
    }

    public void FlecheDroiteDeLap() {
        //On choisit combien de tours il y aura dans la course
        button[0].Play();
        if (nombreDeLAP < 5) {
            nombreDeLAP++;
        } else {
            nombreDeLAP = 1;
        }   
        lapText.text = nombreDeLAP.ToString();
    }

    public void FlecheDroiteDeCarType() {
        //On choisit quels types de voitures seront présents dans la course
        button[0].Play();
        if (indexCarType < 2) {
            indexCarType++;
        } else {
            indexCarType = 0;
        }   
        carTypeText.text = carType[indexCarType];
        //Étant donné que le type ALL implique qu'une voiture de chaque type participe à la course et que nous avons 6 types de véhicules, il est fixé à 6
        if (carType[indexCarType].CompareTo("ALL") == 0) {
            nombreDeJoueur = 6;
            playerText.text = nombreDeJoueur.ToString();
        }
    }

    public void FlecheGaucheDeCarType() {
        //On choisit quels types de voitures seront présents dans la course
        button[0].Play();
        if (indexCarType > 0) {
            indexCarType--;
        } else {
            indexCarType = 2;
        }   
        //Étant donné que le type ALL implique qu'une voiture de chaque type participe à la course et que nous avons 6 types de véhicules, il est fixé à 6
        carTypeText.text = carType[indexCarType];
        if (carType[indexCarType].CompareTo("ALL") == 0) {
            nombreDeJoueur = 6;
            playerText.text = nombreDeJoueur.ToString();
        }
    }

    public void FlecheGaucheDeImage() {
        //On décide quelle carte sera sélectionnée pour la course
        button[0].Play();
        if (carteIndex > 0) {
            cartes[carteIndex].SetActive(false);
            carteIndex--;
            cartes[carteIndex].SetActive(true);
        } else {
            cartes[carteIndex].SetActive(false);
            carteIndex = cartes.Length-1;
            cartes[carteIndex].SetActive(true);
        }
    }

    public void FlecheDroiteDeImage() {
        //On décide quelle carte sera sélectionnée pour la course
        button[0].Play();
        if (carteIndex < cartes.Length-1) {
            cartes[carteIndex].SetActive(false);
            carteIndex++;
            cartes[carteIndex].SetActive(true);
        } else {
            cartes[carteIndex].SetActive(false);
            carteIndex = 0;
            cartes[carteIndex].SetActive(true);
        }
    }

    public void Next() {
        StartCoroutine(Attendez("Next"));
    }

    public void Back() {
        StartCoroutine(Attendez("Back"));
    }

    IEnumerator Attendez(String menu) {
        //La raison pour laquelle j'utilise cela est que, ayant désactivé le menu, le bouton ne produisait pas de son
        button[1].Play();
        yield return new WaitForSeconds(0.3f);
        GameObject.FindWithTag("Menu1").SetActive(false);
        if ("Next" == menu) {
            menuSuivant.SetActive(true);
        } else {
            menuPrecedent.SetActive(true);
        }
    }

}
