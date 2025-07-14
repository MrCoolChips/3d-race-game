// Prenom : Egemen 
// Nom: YAPSIK 
// Date : 31/03/2025
// Objectif du script : Créer un menu où l'on peut voir les modèles de voitures et leurs caractéristiques visuellement. 
//À la fin, enregistrer nos paramètres de jeu et les envoyer au GameManager

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectionDuVoiture : MonoBehaviour
{
    public GameObject[] voitures;
    public Text nomDeLaVoiture;
    public int voitureActive = 0;
    private int valeurMax = 100;
    public Image vitesseFill;
    public Image freinFill;
    public Image accelerationFill;
    public Image durabiliteFill;
    public AudioSource[] button;
    /*
    button[0] = Les Fleches
    button[1] = Start
    */
    public GameObject menuParametresDesCours;
    MenuParametresDesCours parametresDesCours;
    public bool achatDeVoiture;
    AchatDeVoiture magasin;
    public TextMeshProUGUI typeVoiture;
    public TextMeshProUGUI caracteristique;
    public Material[] couleurs;
    public GameObject ecranDeChargement;
    public Slider barreDeChargement;


    void OnEnable()
    {
        //Ce code est utilisé à la fois dans la partie magasin et dans la partie showroom, c’est pourquoi j’ai ajouté une vérification pour le rendre plus compréhensible
        magasin = gameObject.GetComponent<AchatDeVoiture>();
        if (magasin != null) {
            achatDeVoiture = true;
            //S'il s'agit de la partie magasin, il affiche également les informations du magasin
            magasin.InformationDeVoiture(voitures[voitureActive]);
        }

        foreach (var v in voitures) {
            //Si la voiture n'a pas été achetée ou donnée par les développeurs, dansMonGarage reste à false
            if (PlayerPrefs.GetInt("Voiture_" + v.GetComponent<SalleExpositionDeVoitures>().voitureIndex, 0) != 0) {
                v.GetComponent<SalleExpositionDeVoitures>().dansMonGarage = true;
            }  
        }  
        voitures[voitureActive].SetActive(true);
        MettreAJourLesFill();
        parametresDesCours = menuParametresDesCours.GetComponent<MenuParametresDesCours>();
    }

    public void FlecheDroite() {
        //Affiche les voitures de la liste une par une et met à jour leur graphique en fonction de leurs caractéristiques
        button[0].Play();
        //Si le magasin est activé, toutes les voitures sont visibles pour les joueurs, mais dans l'écran de sélection, seules celles qu'ils possèdent apparaissent
        if (achatDeVoiture == true) {
            if (voitureActive != voitures.Length - 1) {
                voitures[voitureActive].SetActive(false);
                voitureActive++;
                voitures[voitureActive].SetActive(true);
            } else {
                voitures[voitureActive].SetActive(false);
                voitureActive = 0;
                voitures[voitureActive].SetActive(true);
            }
                magasin.InformationDeVoiture(voitures[voitureActive]);
        } else {
            bool trouve = false;
            int i = voitureActive;
            while (trouve == false) {
                i++;
                if (i == voitures.Length) {
                    i = 0;
                }
                if (voitures[i].GetComponent<SalleExpositionDeVoitures>().dansMonGarage == true) {
                    trouve = true;
                    voitures[voitureActive].SetActive(false);
                    voitureActive = i;
                    voitures[voitureActive].SetActive(true);
                }
            }
        }
        MettreAJourLesFill();

    }

    public void FlecheGauche() {
        //Affiche les voitures de la liste une par une et met à jour leur graphique en fonction de leurs caractéristiques
        button[0].Play();
        //Si le magasin est activé, toutes les voitures sont visibles pour les joueurs, mais dans l'écran de sélection, seules celles qu'ils possèdent apparaissent.
        if (achatDeVoiture == true) {
            if (voitureActive != 0) {
                voitures[voitureActive].SetActive(false);
                voitureActive--;
                voitures[voitureActive].SetActive(true);
            } else {
                voitures[voitureActive].SetActive(false);
                voitureActive = voitures.Length - 1;
                voitures[voitureActive].SetActive(true);
            }
            magasin.InformationDeVoiture(voitures[voitureActive]);
        } else {
            bool trouve = false;
            int i = voitureActive;
            while (trouve == false) {
                i--;
                if (i == -1) {
                    i = voitures.Length-1;
                }
                if (voitures[i].GetComponent<SalleExpositionDeVoitures>().dansMonGarage == true) {
                    trouve = true;
                    voitures[voitureActive].SetActive(false);
                    voitureActive = i;
                    voitures[voitureActive].SetActive(true);
                }
            }
        }
        MettreAJourLesFill();
    }

    public void Back(GameObject menu) {
        Attendez(menu, transform.parent.gameObject);
    }

    public void MenuUpgrade(GameObject menu) {
        //Ouvre le menu de Upgrade
        if (magasin.prix.text.Equals("OWNED")) {
            PlayerPrefs.SetInt("voitureSelectionne", voitureActive);
            PlayerPrefs.Save();
            Attendez(menu, transform.parent.gameObject);
        } else {
            StartCoroutine(magasin.EcrireLeMessage("You don't own this car"));
        }
    }

    void Attendez(GameObject menu, GameObject menu2) {
        button[1].Play();
        menu2.SetActive(false);
        menu.SetActive(true);
    }

    void MettreAJourLesFill()
    {
        //Met à jour le taux de remplissage et le nom des caractéristiques de la voiture spécifiée
        nomDeLaVoiture.text = voitures[voitureActive].GetComponent<SalleExpositionDeVoitures>().nom;
        vitesseFill.fillAmount = Mathf.Clamp01((float) voitures[voitureActive].GetComponent<SalleExpositionDeVoitures>().vitesse / valeurMax);
        freinFill.fillAmount = Mathf.Clamp01((float) voitures[voitureActive].GetComponent<SalleExpositionDeVoitures>().frein / valeurMax);
        accelerationFill.fillAmount = Mathf.Clamp01((float) voitures[voitureActive].GetComponent<SalleExpositionDeVoitures>().acceleration / valeurMax);
        durabiliteFill.fillAmount = Mathf.Clamp01((float) voitures[voitureActive].GetComponent<SalleExpositionDeVoitures>().durabilite / valeurMax);
        CaracteristiquesDeLaVoiture(voitures[voitureActive].GetComponent<SalleExpositionDeVoitures>().type.ToString());
        voitures[voitureActive].GetComponentInChildren<MeshRenderer>().material = couleurs[PlayerPrefs.GetInt(voitures[voitureActive].name + "Couleur", 0)];
    }

    public void DemarrerLeJeu() {
        button[1].Play();
        //L'index de la voiture sélectionnée est envoyée pour être utilisée dans ControleurDeJeu
        PlayerPrefs.SetInt("voitureSelectionne", voitureActive);
        PlayerPrefs.SetInt("nombreDeJoueur", parametresDesCours.nombreDeJoueur);
        PlayerPrefs.SetInt("nombreDeLAP", parametresDesCours.nombreDeLAP);
        PlayerPrefs.SetInt("indexCarType", parametresDesCours.indexCarType);
        PlayerPrefs.SetInt("Carte", parametresDesCours.carteIndex);
        PlayerPrefs.Save();

        if (parametresDesCours.carteIndex == 0) {
            StartCoroutine(CharglerLeJeu("Carte1"));
        } else if (parametresDesCours.carteIndex == 1) {
            StartCoroutine(CharglerLeJeu("Carte2"));
        }
    }

    IEnumerator CharglerLeJeu(String nom) {
        //Il affiche un écran de chargement jusqu’à ce que le jeu soit prêt, ce qui permet de ne pas commencer le jeu avant que tout soit en place
        AsyncOperation operation = SceneManager.LoadSceneAsync(nom);
        voitures[voitureActive].SetActive(false);
        ecranDeChargement.SetActive(true);
        while (!operation.isDone) {
            float progres = Mathf.Clamp01(operation.progress / .9f);
            barreDeChargement.value = progres;
            yield return null;
        }
    }

    void CaracteristiquesDeLaVoiture(String type) {
        //Affiche les caractéristiques des voitures en fonction de leur type
        if (type == "Typeless") {
            typeVoiture.text = "CAR TYPE: " + type;
            caracteristique.text = "main features: \n<color=#58AA32> + High health </color>\n<color=#FF0000> - Low boost capacity \n - Low speed </color>";
        } else if (type == "Taxi") {
            typeVoiture.text = "CAR TYPE: <color=#daed00>" + type + "</color>";
            caracteristique.text = "<color=#daed00>main features:</color> \n<color=#58AA32> + If the taxi finishes the race earlier than average, it receives a tip \n + If it finishes 1st in the race, the tip is multiplied by 1.5 </color> \n<color=#FF0000> - 20% less effective boost \n - Low boost capacity</color>";
        } else if (type == "SUV") {
            typeVoiture.text = "CAR TYPE: <color=#ff7400>" + type + "</color>";
            caracteristique.text = "<color=#ff7400>main features:</color> \n<color=#58AA32> + High health \n + Good brakes \n + It is not affected by the disadvantages of maps </color> \n<color=#FF0000> - Slow acceleration </color>";
        } else if (type == "Race") {
            typeVoiture.text = "CAR TYPE: <color=#ff4444>" + type + "</color>";
            caracteristique.text = "<color=#ff4444>main features:</color> \n<color=#58AA32> + High speed \n + High acceleration \n + High boost capacity </color> \n<color=#FF0000> - Low health \n - Slow boost refill speed \n - Earn 25% less coin</color>";
        } else if (type == "Police") {
            typeVoiture.text = "CAR TYPE: <color=#1851ff>" + type + "</color>";
            caracteristique.text = "<color=#1851ff>main features:</color> \n<color=#58AA32> + Radar doesn't work for her \n + If there is a vehicle in front of the car, its speed increases by 15% \n + If the car respawns, the boost bar fills up </color> \n<color=#FF0000> - Slow acceleration \n - Low steering angle</color>";
        } else if (type == "Vintage") {
            typeVoiture.text = "CAR TYPE: <color=#e921ff>" + type + "</color>";
            caracteristique.text = "<color=#e921ff>main features:</color> \n<color=#58AA32> + High speed \n + Good Break \n + High boost capacity and fast recharge </color> \n<color=#FF0000> - Low health \n - Low steering angle</color>";
        }
    }
}
