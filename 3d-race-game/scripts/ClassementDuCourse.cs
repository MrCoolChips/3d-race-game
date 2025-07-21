// Prenom : Egemen 
// Nom: YAPSIK 
// Date : 08/04/2025
// Objectif du script :Un script présent sur chaque objet voiture. 
// Il essaie de calculer sa position actuelle dans la course en entrant en collider
// Il calcule le nombre de tours effectués par la voiture sur la circuit

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class ClassementDuCourse : MonoBehaviour
{
    public int pointDeControleActif;
    TimerAndPosition positions;
    public int positionDansLaCourse;
    ControleurDeJeu controleurDeJeu;
    public GameObject dernierPointDeControle;
    public GameObject voiture;
    Rigidbody rb;
    float tempsImmobilite = 0f;
    float minVitesse = 10f;
    float tempsAvantReapparition = 7f;
    TextMeshProUGUI info;
    public int lap;
    private bool peutCompterTour = false;
    public GameObject finish;
    private float raceTime = 0f; 
    public int milliseconds;
    public int minutes;
    public int seconds;
    bool pont;
    public int compteurDeReapparition;
    bool chronometre = true;
    private float tempsLapActuelle;
    public float tempsDuDernierLAP = 0f;
    bool textActif = false;

    void Awake()
    {
        compteurDeReapparition = 0;
        positions = GameObject.FindWithTag("GameController").GetComponent<TimerAndPosition>();
        //Il s'enregistre dans le liste de classement des voitures
        positions.RecevoirVoiture(gameObject, pointDeControleActif);
        finish = GameObject.FindWithTag("Finish"); 
    }
    void Start()
    {
        tempsLapActuelle = 0f;
        lap = 1;
        rb = voiture.GetComponent<Rigidbody>();
        controleurDeJeu = GameObject.FindWithTag("GameController").GetComponent<ControleurDeJeu>();
        info = GameObject.FindWithTag("Info").GetComponent<TextMeshProUGUI>();
    }

    void Update() {
        if (rb != null) {
            //Si la vitesse de la voiture est inférieure à celle de minVitesse, le compteur commence
            if (rb.velocity.magnitude*2.23693629f < minVitesse) {
                tempsImmobilite += Time.deltaTime;
                //Si la durée d'inactivité dépasse un certain temps (tempsAvantReapparition), les voitures peuvent se retrouver au dernier point de contrôle ou au prochain
                if (tempsImmobilite > tempsAvantReapparition)
                {
                    //Si la personne qui remplit ces conditions est le joueur, elle peut appuyer sur la touche F pour revenir au dernier point de contrôle
                    if (gameObject.name == "YOU") {
                        info.text = "If you're stuck, press [F] to respawn the car at the last checkpoint";
                        textActif = true;
                        if (Input.GetKeyDown(KeyCode.F)) {
                            Reapparition();
                            tempsImmobilite = 0f;
                            info.text = "";
                        }
                    } else {
                        //Si la personne qui remplit ces conditions est un adversaire, la voiture sera téléportée à un point target proche
                        dernierPointDeControle = voiture.transform.Find("Helpers/WaypointTargetObject").gameObject;
                        Reapparition();
                        tempsImmobilite = 0f;
                    }
                }
            }
            else {
                //Lorsque la voiture accélère, le texte disparaît et la touche F ne peut plus être utilisée
                tempsImmobilite = 0f;
                if (gameObject.name == "YOU" && textActif == true) {
                    info.text = "";
                    textActif = false;
                }
            }
        }

        if (chronometre) {
            //Chaque véhicule a son propre chronomètre
            raceTime += Time.deltaTime;
            minutes = Mathf.FloorToInt(raceTime / 60f);
            seconds = Mathf.FloorToInt(raceTime % 60f);
            milliseconds = Mathf.FloorToInt((raceTime * 100f) % 100f);
        }

    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("PointDeControle") || other.CompareTag("Finish")) {
            //Lorsqu’il entre en collision avec un collider ayant le tag PointDeControle ou Finish, il met à jour le classement entre les voitures
            pointDeControleActif = int.Parse(other.transform.gameObject.name) + lap*1000;
            positions.MettreAJourLeClassement(gameObject, pointDeControleActif);
            //Il considère le point qu'il a franchi comme son dernier point
            dernierPointDeControle = other.transform.gameObject;
            //Lorsqu’il arrive à la moitié du circuit, il le détecte et active la ligne de fin
            if (!peutCompterTour) {
                if (int.Parse(other.name) == int.Parse(finish.name)/2 ) {
                    peutCompterTour = true;
                }
            }

            pont = false;
        }

        if (other.CompareTag("Finish")) {
            //Lorsqu'il franchit la ligne d'arrivée, s'il est dans le dernier tour, la course se termine ; sinon, "lap" augmente
            if (peutCompterTour) {
                if (gameObject.name == "YOU") {
                    if (lap < PlayerPrefs.GetInt("nombreDeLAP")) {
                        lap++;
                        CalculerLeMeilleurTour();
                        positions.VerifieLAP(lap);
                        peutCompterTour = false;
                    }
                    else {
                        lap++;
                        chronometre = false;
                        CalculerLeMeilleurTour();
                        GameObject.FindWithTag("Colliders").SetActive(false);
                        positions.RecevoirLAP(gameObject);
                        controleurDeJeu.FinDeJeu(positionDansLaCourse);
                    }
                } else {
                    if (lap < PlayerPrefs.GetInt("nombreDeLAP")) {
                        lap++;
                    }
                    else {
                        lap++;
                        positions.RecevoirLAP(gameObject);
                        voiture.SetActive(false);
                    }

                }
            }
        }

        if (other.CompareTag("Vide")) {
            //Si la voiture sort de la piste, par exemple en faisant un tonneau, elle sera réintégrée dans le jeu depuis le dernier point de controle qu'elle a franchi
            //Si un pont est présent sur la carte et que le joueur n'a pas réussi à le franchir, il sera relancé un peu plus en arrière pour lui donner une nouvelle chance d'essayer
            if (pont) {
                //Il apparaît un peu avant le pont
                dernierPointDeControle = GameObject.Find("Colliders/" + (pointDeControleActif%1000+2).ToString());
                pont = false;
            }
            Reapparition();
        }

        if (other.CompareTag("Pont")) {
            pont = true;
        }
    }

    void CalculerLeMeilleurTour() {
        //Une méthode pour calculer le temps du meilleur tour
        tempsDuDernierLAP = raceTime - tempsLapActuelle;
        if (tempsDuDernierLAP < PlayerPrefs.GetFloat("Le_Meilleur_LAP_" + PlayerPrefs.GetInt("Carte"), Mathf.Infinity)) {
            PlayerPrefs.SetFloat("Le_Meilleur_LAP_" + PlayerPrefs.GetInt("Carte"), tempsDuDernierLAP);
            PlayerPrefs.Save();
            StartCoroutine(InfoTexte("NEW RECORD BEST LAP SCORE: " + Mathf.FloorToInt(tempsDuDernierLAP / 60f) + ":" + Mathf.FloorToInt(tempsDuDernierLAP % 60f) + ":"+ Mathf.FloorToInt((tempsDuDernierLAP * 100f) % 100f), 4f));
        }
        tempsLapActuelle = raceTime;
    }

    IEnumerator InfoTexte(String message, float temps) {
        // Affiche un message pendant une durée limitée
        info.text = message;
        yield return new WaitForSeconds(temps);
        info.text = "";
    }

    public void Reapparition() {
        //Il téléporte la voiture au dernier point de contrôle
        float taille = 0;
        compteurDeReapparition++;
        BoostDePolice();
        if (dernierPointDeControle != null) {
            //Comme je ne voulais pas que les voitures apparaissent directement au centre de la carte, j'ai fait en sorte qu'elles arrivent légèrement d'une hauteur
            if (dernierPointDeControle.GetComponent<Collider>() != null) {
                taille = dernierPointDeControle.GetComponent<Collider>().bounds.size.y/2;
            }
            voiture.transform.position = dernierPointDeControle.transform.position + new Vector3(0f, taille + 1f, 0f);
            voiture.transform.rotation = dernierPointDeControle.transform.rotation;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.velocity = transform.forward * 3f;
        } else {
            //Si le dernier point de contrôle n'existe pas, il retourne au point de départ de la course
            voiture.transform.position = new Vector3(0f, 3f, 0f);
        }
    }

    void BoostDePolice() {
        // Lorsque la voiture du joueur réapparaît, si c'est un véhicule de police, elle renaît avec la barre de boost au maximum
        if (gameObject.name == "YOU") {
            CarController vehicule =  voiture.GetComponent<CarController>();
            if ((int) vehicule.type == 3) {
                vehicule.valeurBoost = vehicule.maxboost;
            }
        }
    }
}
