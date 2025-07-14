using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AchatDeVoiture : MonoBehaviour
{
    public TextMeshProUGUI prix;
    public TextMeshProUGUI portefeuille;
    public TextMeshProUGUI message;
    public GameObject selectionDuVoiture;
    bool modeAffichage = false;
    GameObject voiture;
    float rotationX = 0f;
    public AudioSource[] sons;
    /*
    sons[0] = Acheter;
    sons[1] = Erreur;
    sons[2] = View Car;
    */
    
    void OnEnable()
    {
        portefeuille.text = PlayerPrefs.GetInt("Coin", 0).ToString() + "<sprite=0>";
    }

    void Update() {
        if (modeAffichage) {
            float mouseX = Input.GetAxis("Mouse X") * 5f;
            float mouseY = Input.GetAxis("Mouse Y") * 5f;
            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -45f, 45f);
            voiture.transform.localEulerAngles = new Vector3(rotationX, voiture.transform.localEulerAngles.y, 0);

            voiture.transform.Rotate(-mouseY, mouseX, 0);
        }
    }

    public void InformationDeVoiture(GameObject vehicule) {
        voiture = vehicule;
        SalleExpositionDeVoitures informations = vehicule.GetComponent<SalleExpositionDeVoitures>();
        if (informations.dansMonGarage == true) {
            prix.text = "OWNED";
        } else {
            prix.text = "BUY FOR " + informations.prix + "<sprite=0>";
        }
    }

    public void AcheterLaVoiture() {
        SalleExpositionDeVoitures informations = voiture.GetComponent<SalleExpositionDeVoitures>();
        if (informations.dansMonGarage == false) {
            if (PlayerPrefs.GetInt("Coin", 0) >= informations.prix) {
                sons[0].Play();
                int coin = PlayerPrefs.GetInt("Coin", 0) -  informations.prix;
                PlayerPrefs.SetInt("Coin", coin);
                portefeuille.text = coin.ToString() + "<sprite=0>";
                informations.dansMonGarage = true;
                informations.GetComponent<SalleExpositionDeVoitures>().dansMonGarage = true;
                selectionDuVoiture.GetComponent<SelectionDuVoiture>().voitures[informations.voitureIndex].GetComponent<SalleExpositionDeVoitures>().dansMonGarage = true;
                PlayerPrefs.SetInt("Voiture_" + informations.voitureIndex, 1);
                PlayerPrefs.Save();
                prix.text = "OWNED";
                StartCoroutine(EcrireLeMessage("Your purchase was successful"));
            } else {
                sons[1].Play();
                StartCoroutine(EcrireLeMessage("You don't have enough money"));
            }
        } else {
            sons[1].Play();
            StartCoroutine(EcrireLeMessage("You already own this vehicle"));
        }
    }

    public IEnumerator EcrireLeMessage(String m) {
        message.text = m;
        yield return new WaitForSeconds(5f);
        message.text = "";
    }

    public void voirLaVoiture() {
        sons[2].Play();
        if (modeAffichage) {
            voiture.GetComponent<SalleExpositionDeVoitures>().retourne = false;
            modeAffichage = false;
            voiture.transform.rotation = Quaternion.Euler(0, 0, 0);
        } else {
            voiture.GetComponent<SalleExpositionDeVoitures>().retourne = true;
            modeAffichage = true;
        }
    }
    
}
