// Prenom : Egemen 
// Nom: YAPSIK 
// Date : 04/04/2025
// Objectif du script : La partie qui gère les règles du jeu et son déroulement à partir des informations reçues depuis les menus

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Vehicles.Car;

public class ControleurDeJeu : MonoBehaviour
{

    //
    public GameObject[] voitures;
    public GameObject[] adversaires;
    public GameObject[] pointDeDepart;
    GameObject joueur;
    String[] typeDeCourse = {"RANDOM", "SAME", "ALL"};
    public GameObject finDeCourse;
    public GameObject AnimationDeFinDeCourse;
    int nombreDeJoueur;
    public AudioSource[] sons;
    public TextMeshProUGUI argent;
    /*
    sons[0] = victoire
    sons[1] = defaite
    */
    public Material[] couleurs;

    void Start()
    {
        DemarrerLeJeu();   
    }

    void DemarrerLeJeu() {
        /*La classe SelectionDuVoiture possède une liste de voitures dans le même ordre qu’une autre liste identique. 
        En récupérant l’index de la voiture sélectionnée avec voitureSelectionne, 
        cela nous permet de générer la voiture correspondante dans la partie course*/
        int i = 0;
        nombreDeJoueur =  PlayerPrefs.GetInt("nombreDeJoueur");
        int voitureSelectionne = PlayerPrefs.GetInt("voitureSelectionne");
        //On détermine aléatoirement à quelle position notre voiture commencera la course
        int positionDuJoueur = UnityEngine.Random.Range(0,nombreDeJoueur);

        if (typeDeCourse[PlayerPrefs.GetInt("indexCarType")] == "RANDOM") {
            //Il sélectionnera les types de véhicules de manière aléatoire et créera autant d'adversaires que le nombre de joueurs choisis
            joueur = Instantiate(voitures[voitureSelectionne], pointDeDepart[positionDuJoueur].transform.position, pointDeDepart[positionDuJoueur].transform.rotation);
            joueur.transform.Find("Body").GetComponent<MeshRenderer>().material = couleurs[PlayerPrefs.GetInt(joueur.name.Split("(")[0] + "Couleur", 0)];
            for (i = 0; i < nombreDeJoueur; i++) {
                
                if (i != positionDuJoueur) {
                    
                    int random = UnityEngine.Random.Range(0, adversaires.Length);
                    GameObject adversaire = Instantiate(adversaires[random], pointDeDepart[i].transform.position, pointDeDepart[i].transform.rotation);
                    adversaire.GetComponent<AdversaireController>().indexTrajetDeComputer = i;
                }
            }
        } else if (typeDeCourse[PlayerPrefs.GetInt("indexCarType")] == "SAME") {
            //Il sélectionnera, un par un, les voitures du même type que la nôtre
            joueur = Instantiate(voitures[voitureSelectionne], pointDeDepart[positionDuJoueur].transform.position, pointDeDepart[positionDuJoueur].transform.rotation);
            joueur.transform.Find("Body").GetComponent<MeshRenderer>().material = couleurs[PlayerPrefs.GetInt(joueur.name.Split("(")[0] + "Couleur", 0)];
            int j = 0;

            while (i < nombreDeJoueur) {

                while (adversaires[j].GetComponent<AdversaireController>().type.ToString() != joueur.GetComponent<CarController>().type.ToString() ) {
                    j++;
                    if (j >= adversaires.Length) {
                        j = 0;
                    }
                }

                if (i == positionDuJoueur) {
                    i++;
                } else {
                    GameObject adversaire = Instantiate(adversaires[j], pointDeDepart[i].transform.position, pointDeDepart[i].transform.rotation);
                    adversaire.GetComponent<AdversaireController>().indexTrajetDeComputer = i;
                    i++;
                    j++;
                    if (j >= adversaires.Length) {
                        j = 0;
                    }
                }
            }
        } else if (typeDeCourse[PlayerPrefs.GetInt("indexCarType")] == "ALL") {
            //Il ajoutera les 6 types de voitures présents dans le jeu à la course
            joueur = Instantiate(voitures[voitureSelectionne], pointDeDepart[positionDuJoueur].transform.position, pointDeDepart[positionDuJoueur].transform.rotation);
            joueur.transform.Find("Body").GetComponent<MeshRenderer>().material = couleurs[PlayerPrefs.GetInt(joueur.name.Split("(")[0] + "Couleur", 0)];
            String[] listesDeTypes = { "Typeless", "Taxi", "SUV", "Race", "Police", "Vintage" };
            int positionList = 0;
            string joueurType = joueur.GetComponent<CarController>().type.ToString();

            while (i < nombreDeJoueur) {
                if (i == positionDuJoueur) {
                    i++;
                    continue;
                }

                //On fait une vérification pour éviter que le type de voiture que nous avons choisi n'apparaisse de nouveau
                if (!listesDeTypes[positionList].Equals(joueurType, StringComparison.OrdinalIgnoreCase)) {
                    int j = 0;
                    int essaisMax = adversaires.Length;
                    bool trouve = false;

                    while (essaisMax > 0) {
                        //
                        string adversaireType = adversaires[j].GetComponent<AdversaireController>().type.ToString();
                        if (listesDeTypes[positionList].Equals(adversaireType, StringComparison.OrdinalIgnoreCase)) {
                            trouve = true;
                            break;
                        }
                        //Cela permet de parcourir la liste en boucle sans dépasser sa longueur 
                        j = (j + 1) % adversaires.Length;
                        essaisMax--;
                    }

                    if (trouve) {
                        GameObject adversaire = Instantiate(adversaires[j], pointDeDepart[i].transform.position, pointDeDepart[i].transform.rotation);
                        adversaire.GetComponent<AdversaireController>().indexTrajetDeComputer = i;
                        i++;
                    }
                }

                positionList++;
            }
        }
        
        //Cela permet de lier la voiture créée aux objets de la caméra
        GameObject.FindWithTag("MainCamera").GetComponent<ControleurDeCamera>().objectif[0] = joueur.transform.Find("Camera/positionCamera");
        GameObject.FindWithTag("MainCamera").GetComponent<ControleurDeCamera>().objectif[1] = joueur.transform.Find("Camera/angleCamera");
        //cameras[0] est le MainCamera, Il est déjà enregistré
        GameObject.FindWithTag("GameController").GetComponent<TransitionDeCamera>().cameras[1] = joueur.transform.Find("Camera/cameraArriere").gameObject;
        GameObject.FindWithTag("GameController").GetComponent<TransitionDeCamera>().cameras[2] = joueur.transform.Find("Camera/cameraFrontale").gameObject;
        GameObject.FindWithTag("GameController").GetComponent<TransitionDeCamera>().cameras[3] = joueur.transform.Find("Camera/cameraAction").gameObject;
        //Les fonctionnalités qui lient automatiquement les autres GameObjects de notre voiture entre eux se trouvent dans CarController
    }

    public void FinDeJeu(int position) {
        //Un menu qui affiche notre position et combien d’argent a été gagné à la fin de la course
        GameObject.FindWithTag("GameController").GetComponent<TransitionDeCamera>().AjusterLaCamera(1);
        if (position <= nombreDeJoueur/2) {
            sons[0].Play();
            AnimationDeFinDeCourse.transform.Find("Classement").GetComponent<TextMeshProUGUI>().text = "VICTORY #"+ position;
            finDeCourse.transform.Find("Titre").GetComponent<TextMeshProUGUI>().text = "<color=#58AA32>VICTORY #"+ position + "</color>";
            StartCoroutine(AnimationDeFin(position));

        } else {
            sons[1].Play();
            AnimationDeFinDeCourse.transform.Find("Classement").GetComponent<TextMeshProUGUI>().text = "<color=#FF0000>DEFEAT #"+ position + "</color>";
            finDeCourse.transform.Find("Titre").GetComponent<TextMeshProUGUI>().text = "<color=#FF0000>DEFEAT #"+ position + "</color>";
            StartCoroutine(AnimationDeFin(position));
        }
    }

    IEnumerator AnimationDeFin(int position) {
        //Il joue l’animation de fin de course, puis affiche l’écran de fin
        GameObject.FindWithTag("IndicateurDeVitesse").SetActive(false);
        GameObject.FindWithTag("MenudeClassement").SetActive(false);
        AnimationDeFinDeCourse.SetActive(true);
        yield return new WaitForSeconds(8.5f);
        AnimationDeFinDeCourse.SetActive(false);
        AudioListener.volume = 0f;
        Time.timeScale = 0f; 
        finDeCourse.SetActive(true);
        gameObject.GetComponent<TimerAndPosition>().ClassementPourFinDeJeu();
        DistributionDeLArgent(position);
    }

    public void DistributionDeLArgent(int position) {
        //Il affiche combien d’argent a été gagné à la fin de la course
        int[] tarifs = {50, 40, 30, 20, 10, 0};
        int total = 0;
        PlayerPrefs.SetInt("nombreDeJeux", PlayerPrefs.GetInt("nombreDeJeux", 0) + 1);
        argent.text = "RANKING BONUS" + "    +" + ((nombreDeJoueur - position)* 10).ToString() +	"<sprite=0>";
        total += (nombreDeJoueur - position)* 10;
        //Si nous terminons la course après avoir dépassé la moitié du nombre total de joueurs, nous gagnons de la coin. Sinon, nous ne pouvons pas obtenir certains bonus en coin
        if (position <= nombreDeJoueur/2) {
            PlayerPrefs.SetInt("victoire",  PlayerPrefs.GetInt("victoire", 0) +1);
            argent.text += "<br>LAP BONUS" + "    +" + (PlayerPrefs.GetInt("nombreDeLAP")* 10).ToString() +	"<sprite=0>";
            total += PlayerPrefs.GetInt("nombreDeLAP")* 10;
            argent.text += "<br>OPPONENT CAR TYPE WAS " +  typeDeCourse[PlayerPrefs.GetInt("indexCarType")] + "    +" + (tarifs[PlayerPrefs.GetInt("indexCarType")] -20).ToString() + "<sprite=0>";
            total += tarifs[PlayerPrefs.GetInt("indexCarType")] -20;
            argent.text += "<br>PLAYER CAR TYPE WAS " +  joueur.GetComponent<CarController>().type + "    +" + tarifs[(int)joueur.GetComponent<CarController>().type].ToString() + "<sprite=0>";
            total += tarifs[(int)joueur.GetComponent<CarController>().type];

            if ((int) joueur.GetComponent<CarController>().type == 1) {
            // Si la voiture est de type "taxi" et qu'elle a gagné la course, le client laisse un pourboire compris entre 10 et 100.
            // Si le taxi termine la course à la 1ère place, le pourboire est multiplié par 1,5.
            int tip = UnityEngine.Random.Range(10, 101);
            if (nombreDeJoueur - position == 1) {
                tip += tip/2;
            }
            argent.text += "<br><color=#F8FF00>TIP +"+ tip + "</color><sprite=0>";
            total += tip;
            }
        } else {
            PlayerPrefs.SetInt("defaite", PlayerPrefs.GetInt("defaite", 0)+1);
            argent.text += "<br>LAP BONUS    <color=#FF0000>+0</color><sprite=0>";
            argent.text += "<br>OPPONENT CAR TYPE WAS " +  typeDeCourse[PlayerPrefs.GetInt("indexCarType")] + "    <color=#FF0000>+0</color>" + "<sprite=0>";
            argent.text += "<br>PLAYER CAR TYPE WAS " +  joueur.GetComponent<CarController>().type + "    <color=#FF0000>+0</color><sprite=0>";
        }
        argent.text += "<br>360 BONUS" + "    +" + (joueur.GetComponent<CarController>().combo360*5).ToString() +	"<sprite=0>";
        total+= joueur.GetComponent<CarController>().combo360*5;
        argent.text += "<br>RESPAWN PENALTY" + "    <color=#FF0000>-" + joueur.GetComponentInChildren<ClassementDuCourse>().compteurDeReapparition + "</color><sprite=0>";
        total -= joueur.GetComponentInChildren<ClassementDuCourse>().compteurDeReapparition;

        if ((int) joueur.GetComponent<CarController>().type == 5) {
            //Si le type de la voiture est "course", une taxe de 25 % est appliquée
            argent.text += "<br>ENVIRONMENTAL POLLUTION TAX" + "    <color=#FF0000>-" + total/4 + "</color><sprite=0>";
            total -= total/4;
        }
        //Nous n'autorisons pas que notre solde soit négatif
        if (total > 0) {
            argent.text += "<br>TOTAL   +" + total + "<sprite=0>";
        } else{
            total = 0;
            argent.text += "<br>TOTAL   <color=#FF0000>" + total + "</color><sprite=0>";
        }
        PlayerPrefs.SetInt("totalCoin", PlayerPrefs.GetInt("totalCoin", 0) + total);
        PlayerPrefs.SetInt("VoitureType_" + (int)joueur.GetComponent<CarController>().type,  PlayerPrefs.GetInt("VoitureType_" + (int)joueur.GetComponent<CarController>().type, 0)+1);
        PlayerPrefs.Save();
        AjouterCoin(total);
    }

    public void AjouterCoin(int montant)
    {
        //Récupère le nombre actuel de coin (0 par défaut si aucune valeur n'existe)
        int coinActuelles = PlayerPrefs.GetInt("Coin", 0);
        int nouveauTotal = coinActuelles + montant;

        //Empêche le coin de devenir négatif
        nouveauTotal = Mathf.Max(0, nouveauTotal);

        //Sauvegarde le nouveau total dans
        PlayerPrefs.SetInt("Coin", nouveauTotal);
        PlayerPrefs.Save();
    }

    public void Rematch() {
        //Un bouton qui renvoie à l’écran de sélection des voitures
        foreach (var v in gameObject.GetComponent<TimerAndPosition>().cars) {
            Destroy(v.voiture);
        }
        AudioListener.volume = 1f;
        Time.timeScale = 1f;
        Destroy(joueur); 
        SceneManager.LoadScene("Carte" + (PlayerPrefs.GetInt("Carte")+1).ToString());
    }

    public void MainMenu() {
        //Un bouton qui renvoie à l’écran de menu principale
        foreach (var v in gameObject.GetComponent<TimerAndPosition>().cars) {
            Destroy(v.voiture);
        }
        AudioListener.volume = 1f;
        Time.timeScale = 1f;
        Destroy(joueur); 
        SceneManager.LoadScene("MenuManager");
    }
}