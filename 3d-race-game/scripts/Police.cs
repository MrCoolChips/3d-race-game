// Prenom : Egemen 
// Nom: YAPSIK 
// Date : 02/05/2025
// Objectif du script : Appliquer les caractéristiques des véhicules de type "police"

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class Police : MonoBehaviour
{
    public GameObject[] lumieres;  
    public AudioSource sirene;
    private CarController carController;
    private float bonusVitesse;
    private float limiteDeDetection = 30f;
    private bool allume = false; 
    private GameObject adversaire;
    private float distance;
    public GameObject classement;
    ClassementDuCourse classementDuCourse;

    void Start() {
        classementDuCourse = classement.GetComponent<ClassementDuCourse>();
        carController = gameObject.GetComponentInParent<CarController>();
        // Lorsque le mode poursuite est actif, la voiture reçoit une augmentation de vitesse de 15 %
        bonusVitesse = carController.EnvoyerLaVitesseMaximale() * 0.15f;
    }

    void Update() {
        ModePoursuite();
    }

    void ModePoursuite() {
        // Si le joueur se rapproche d'un adversaire dans le classement, le mode poursuite est activé
        // Par exemple, si le joueur est en 3e position, il cible la voiture en 2e position
        if (classementDuCourse.positionDansLaCourse-1 != -1) {
            adversaire = GameObject.FindGameObjectWithTag("GameController").GetComponent<TimerAndPosition>().cars[classementDuCourse.positionDansLaCourse-1].voiture;
        } else {
            // Si la voiture est en 1ère position, ce mode ne s'active pas
            adversaire = null;
            Desactiver();
        }

        if (adversaire != null) {
            // Il calcule la distance entre la voiture du joueur et la voiture cible
            distance = Vector3.Distance(transform.position, adversaire.transform.position);

            if (distance <= limiteDeDetection) {
                Activer();
            }
            else {
                Desactiver();
            }
        }
    }

    void Activer() {
        // Allume les sirènes et les lumières de la voiture et active le boost de vitesse
        allume = true;
        if (!sirene.isPlaying) {
            carController.AugmenterLaVitesseMaximale(bonusVitesse);
            StartCoroutine(LumieresDePolice());
            sirene.Play();
        }
    }

    void Desactiver() {
        // Désactive les sirènes et les lumières de la voiture, puis enlève le boost de vitesse
        allume = false;
        foreach (var lum in lumieres) {
            lum.SetActive(false); 
        }
        if (sirene.isPlaying) {
            carController.AugmenterLaVitesseMaximale(-bonusVitesse);
            sirene.Stop();
        }
    }

    private IEnumerator LumieresDePolice() {
        // Active la lumière rouge et la lumière bleue pour créer l'ambiance
        while (allume) {
            lumieres[0].SetActive(true); 
            lumieres[1].SetActive(false); 

            yield return new WaitForSeconds(0.5f);

            lumieres[1].SetActive(true); 
            lumieres[0].SetActive(false); 

            yield return new WaitForSeconds(0.5f);
        }
    }
}
