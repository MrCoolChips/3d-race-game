// Prenom : Egemen 
// Nom: YAPSIK 
// Date : 27/04/2025
// Objectif du script : Un menu "Upgrade" qui permet de modifier les voitures et de changer leur couleur

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public GameObject[] voituresPhysiques;
    public TextMeshProUGUI portefeuille;
    public GameObject[] menuDUpgrade;
    int menuActif;
    SalleExpositionDeVoitures voiture;
    public GameObject menuDAchat;
    GameObject upgradeSelectionee;
    int argentDUpgrade;
    public Material[] couleurs;
    string nom;
    public AudioSource[] sons;
    
    void OnEnable()
    {
        menuActif = 0;
        portefeuille.text = PlayerPrefs.GetInt("Coin", 0)+"<sprite=0>";
        voituresPhysiques[PlayerPrefs.GetInt("voitureSelectionne", 0)].SetActive(true);
        nom = voituresPhysiques[PlayerPrefs.GetInt("voitureSelectionne", 0)].name;
        voiture = voituresPhysiques[PlayerPrefs.GetInt("voitureSelectionne", 0)].GetComponent<SalleExpositionDeVoitures>();
        voituresPhysiques[PlayerPrefs.GetInt("voitureSelectionne", 0)].GetComponentInChildren<MeshRenderer>().material = couleurs[PlayerPrefs.GetInt(nom + "Couleur", 0)];
        /* 
        category 0 = Engine Upgrade
        category 1 = Brake Upgrade
        category 2 = Turbo Upgrade
        category 3 = Acceleration Upgrade
        category 4 = Color change

        upgrade 0 = Upgrade LVL 0
        upgrade 1 = Upgrade LVL 1
        upgrade 2 = Upgrade LVL 2
        upgrade 3 = Upgrade LVL 3
        upgrade 4 = Upgrade LVL 4
        */ 
        for (int category = 0; category < voiture.ameliorations.Length; category++) {
            //Si un "upgrade" dans les catégories spécifiées a été acheté pour cette voiture, il sera marqué
            for (int upgrade = 0; upgrade < voiture.ameliorations[category].Length; upgrade++) {
                if (PlayerPrefs.GetInt(nom + "_upgrade_" + category + "_" + upgrade, 0) == 1 || upgrade == 0) {
                    voiture.ameliorations[category][upgrade] = true;
                }
            }
        }
    }

    public void ouvrirLeMenu(GameObject menu) {
        //Ferme le menu principal des upgrades et ouvre le sous-menu des upgrades
        sons[0].Play();
        menuDUpgrade[0].SetActive(false);
        AffichageDeModification(menu);
    }

    public void Back(GameObject menu) {
        //Si le menu principal des upgrades est actif, il revient à la page du magasin
        //Si le sous-menu des upgrades est ouvert, il retourne au menu principal des upgrades
        sons[3].Play();
        if (menuDUpgrade[0].activeSelf) {
            voituresPhysiques[PlayerPrefs.GetInt("voitureSelectionne", 0)].SetActive(false); 
            transform.parent.gameObject.SetActive(false);
            menu.SetActive(true);
        } else {
            foreach (Transform enfant in menuDUpgrade[menuActif].transform) {
                enfant.gameObject.SetActive(false);
            }
            menuDUpgrade[menuActif].SetActive(false);
            menuDUpgrade[0].SetActive(true);
        }
        menuDAchat.SetActive(false);
    }


    void AffichageDeModification(GameObject menu) {
        //Lorsque le sous-menu des upgrades est ouvert, il affiche quels upgrades ont été achetés précédemment, 
        // c'est-à-dire ceux qui sont marqués comme "OWNED", ainsi que l'upgrade sélectionné ("SELECTED") ou le prix de l'upgrade.
        bool[] tableau = null;
        int indice = int.Parse(menu.tag);
        int selectionne = PlayerPrefs.GetInt(nom + "_upgrade_selectionne_" + indice, 0);
        double bonus = 1.0;

        switch (menu.tag) {
            //J'ai pensé que chaque upgrade devrait avoir un prix différent, car je ne considère pas les upgrades de moteur et de freins comme équivalents
            case "0":
            tableau = voiture.ameliorations[indice];
            menuActif = 1;
            bonus = 2.0;
            break;

            case "1":
            tableau = voiture.ameliorations[indice];
            menuActif = 2;
            bonus = 1.25;
            break;

            case "2":
            tableau = voiture.ameliorations[indice];
            menuActif = 3;
            bonus = 1.5;
            break;

            case "3":
            tableau = voiture.ameliorations[indice];
            menuActif = 4;
            bonus = 1.75;
            break;

            case "4":
            tableau = voiture.ameliorations[indice];
            menuActif = 5;
            break;
            
            default:
            Debug.LogWarning("Nom de menu inconnu : " + menu.name);
            return;
        }

        if (tableau != null && tableau.Length > 0) {
            //Ici, les états de la voiture sont définis (« SELECTED », « OWNED », etc.)
            for (int i = 0; i < tableau.Length; i++) {
                menu.transform.GetChild(i).gameObject.SetActive(true);
                if (i == selectionne) {
                    menu.transform.GetChild(i).Find("Situation").GetComponent<TextMeshProUGUI>().text = "SELECTED";
                } else if (tableau[i] == true) {
                    menu.transform.GetChild(i).Find("Situation").GetComponent<TextMeshProUGUI>().text = "OWNED";
                } else {
                    if (menu.name == "AmeliorationsDuCouleur") {
                        //Tous les prix des couleurs sont fixes
                        menu.transform.GetChild(i).Find("Situation").GetComponent<TextMeshProUGUI>().text = 300 + "<sprite=0>";
                    } else {
                        menu.transform.GetChild(i).Find("Situation").GetComponent<TextMeshProUGUI>().text = (((int) voiture.type + 1) * bonus * 100 * i).ToString() + "<sprite=0>";
                    }
                }
            }
            menu.SetActive(true);
        }
    }

    public void ObtenirUpgrade(GameObject upgrade) {
        //C'est ici que l'on choisit les modifications de la voiture et où les achats sont effectués
        string situation = upgrade.transform.Find("Situation").GetComponent<TextMeshProUGUI>().text;
        if (situation.Equals("OWNED")) {
            sons[1].Play();
            //Nous transformons l'élément marqué comme "SELECTED" en "OWNED"
            GameObject.FindWithTag(PlayerPrefs.GetInt(nom + "_upgrade_selectionne_" + upgrade.transform.parent.tag, 0).ToString()).transform.Find("Situation").GetComponent<TextMeshProUGUI>().text = "OWNED";
            PlayerPrefs.SetInt(nom + "_upgrade_selectionne_" + upgrade.transform.parent.tag, int.Parse(upgrade.tag));
            PlayerPrefs.Save();
            upgrade.transform.Find("Situation").GetComponent<TextMeshProUGUI>().text = "SELECTED";
            if (upgrade.transform.parent.tag == "4") {
                //Si c'est la partie pour changer la couleur, nous la modifions immédiatement
                ChangerDeCouleur(int.Parse(upgrade.tag));
            }
        } else if (!situation.Equals("SELECTED")) {
            //Un menu s'affiche pour vous demander si vous etes sûr de votre achat
            menuDAchat.transform.Find("Message").GetComponent<TextMeshProUGUI>().text = "Are you sure you want to buy " + upgrade.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text + " for " + situation + "   ?";
            menuDAchat.SetActive(true);
            upgradeSelectionee = upgrade;
            if (upgrade.transform.parent.tag == "4") {
                //Lors du processus d'achat, la couleur sélectionnée est appliquée pour prévisualisation. Cependant, si l'achat est annulé, la voiture retrouve sa couleur d'origine
                voituresPhysiques[PlayerPrefs.GetInt("voitureSelectionne", 0)].GetComponentInChildren<MeshRenderer>().material = couleurs[int.Parse(upgrade.tag)];
            }
            argentDUpgrade = int.Parse(situation.Split("<")[0]);
        }
    }

    public void Repondre(bool reponse) {
        //L'action est effectuée en fonction de la réponse du processus d'achat
        if (reponse) {
            int coin = PlayerPrefs.GetInt("Coin", 0);
            if (coin >= argentDUpgrade) {
                sons[2].Play();
                PlayerPrefs.SetInt("Coin", coin - argentDUpgrade);
                portefeuille.text = coin - argentDUpgrade+"<sprite=0>";
                GameObject.FindWithTag(PlayerPrefs.GetInt(nom + "_upgrade_selectionne_" + upgradeSelectionee.transform.parent.tag, 0).ToString()).transform.Find("Situation").GetComponent<TextMeshProUGUI>().text = "OWNED";
                PlayerPrefs.SetInt(nom + "_upgrade_selectionne_" + upgradeSelectionee.transform.parent.tag, int.Parse(upgradeSelectionee.tag));
                upgradeSelectionee.transform.Find("Situation").GetComponent<TextMeshProUGUI>().text = "SELECTED";
                //Les noms des couleurs ne sont pas écrit, ce qui permet de les voir même sans les avoir achetées
                if (upgradeSelectionee.transform.parent.tag == "4") {
                    ChangerDeCouleur(int.Parse(upgradeSelectionee.tag));
                }
                PlayerPrefs.SetInt(nom + "_upgrade_" + upgradeSelectionee.transform.parent.tag + "_" + upgradeSelectionee.tag, 1);
                PlayerPrefs.Save();
                upgradeSelectionee = null;
                menuDAchat.SetActive(false);
            }
        } else {
            if (upgradeSelectionee.transform.parent.tag == "4") {
                //Lors du processus d'achat, la couleur sélectionnée est appliquée pour prévisualisation. Cependant, si l'achat est annulé, la voiture retrouve sa couleur d'origine
                voituresPhysiques[PlayerPrefs.GetInt("voitureSelectionne", 0)].GetComponentInChildren<MeshRenderer>().material = couleurs[PlayerPrefs.GetInt(nom + "_upgrade_selectionne_4", 0)];
            }
            menuDAchat.SetActive(false);
        }
    }

    void ChangerDeCouleur(int couleur) {
        //La couleur de la voiture est modifiée et enregistrée
        GameObject vehicule = voituresPhysiques[PlayerPrefs.GetInt("voitureSelectionne", 0)];
        vehicule.GetComponentInChildren<MeshRenderer>().material = couleurs[couleur];
        PlayerPrefs.SetInt(nom + "Couleur", couleur);
        PlayerPrefs.Save();

    } 
    
}
