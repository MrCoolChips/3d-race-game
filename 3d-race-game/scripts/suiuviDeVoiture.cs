using UnityEngine;

// Ce script permet de suivre la progression d'une voiture sur un circuit à partir de points de passage
public class SuiviVoiture : MonoBehaviour
{
    // Tableau des points de passage à définir sur le circuit
    public Transform[] pointsPassage;

    // Index du point de passage actuel
    private int indexPointActuel = 0;

    // Distance totale parcourue par la voiture
    private float distanceTotaleParcourue = 0f;

    void Update()
    {
        ActualiserProgression();
    }

    // Actualise la progression en vérifiant si la voiture dépasse le prochain point de passage
    void ActualiserProgression()
    {
        Transform pointActuel = pointsPassage[indexPointActuel];
        Transform pointSuivant = pointsPassage[(indexPointActuel + 1) % pointsPassage.Length];

        // Calcule la distance entre la voiture et le prochain point de passage
        float distanceAuPointSuivant = Vector3.Distance(transform.position, pointSuivant.position);

        // Si la voiture a dépassé le prochain point de passage, on met à jour l'index et la distance parcourue
        if (distanceAuPointSuivant < Vector3.Distance(pointActuel.position, pointSuivant.position))
        {
            indexPointActuel = (indexPointActuel + 1) % pointsPassage.Length;
            distanceTotaleParcourue += Vector3.Distance(pointActuel.position, pointSuivant.position);
        }
    }

    // Renvoie la progression totale de la voiture sur le circuit
    public float ObtenirProgressionTotale()
    {
        Transform pointSuivant = pointsPassage[(indexPointActuel + 1) % pointsPassage.Length];
        float distanceAuPointSuivant = Vector3.Distance(transform.position, pointSuivant.position);

        return distanceTotaleParcourue - distanceAuPointSuivant;
    }
}