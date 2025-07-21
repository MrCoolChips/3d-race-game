using UnityEngine;
using UnityEngine.UI;

// Script pour gérer les tours (laps) du joueur
public class GestionTours : MonoBehaviour
{
    // Tableau des points de passage obligatoires sur le circuit
    public Transform[] pointsPassage;

    // Texte pour afficher le nombre de tours
    public Text texteTours;

    // Nombre total de tours à réaliser
    public int toursTotaux = 3;

    private int tourActuel = 0;
    private int prochainPointPassage = 0;

    void Start()
    {
        ActualiserAffichageTours();
    }

    void OnTriggerEnter(Collider other)
    {
        // Vérifie si le joueur entre dans un point de passage
        if (other.transform == pointsPassage[prochainPointPassage])
        {
            prochainPointPassage++;

            // Si tous les points de passage sont validés, un tour est complété
            if (prochainPointPassage >= pointsPassage.Length)
            {
                ValiderTour();
                prochainPointPassage = 0;
            }
        }
    }

    void ValiderTour()
    {
        tourActuel++;
        ActualiserAffichageTours();

        // Vérifie si la course est terminée
        if (tourActuel >= toursTotaux)
        {
            // Actions à faire à la fin de la course
            Debug.Log("Course terminée !");
        }
    }

    void ActualiserAffichageTours()
    {
        texteTours.text = "Tour : " + tourActuel + " / " + toursTotaux;
    }
}