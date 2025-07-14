using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

// Ce script gère l'affichage du chrono et la position du joueur durant la course
public class ChronoEtPosition : MonoBehaviour
{
    // Texte affichant le chrono à l'écran
    public Text texteChrono;

    // Texte affichant la position du joueur à l'écran
    public Text textePosition;

    // Liste des points de passage placés dans l'ordre du circuit
    public List<Transform> pointsPassage;

    // Liste des voitures avec chacune leur script "SuiviVoiture"
    public List<SuiviVoiture> voitures;

    // Script "SuiviVoiture" associé à la voiture du joueur
    public SuiviVoiture voitureJoueur;

    private float tempsCourse;

    void Update()
    {
        ActualiserChrono();
        ActualiserPositions();
    }

    // Met à jour le chronomètre affiché
    void ActualiserChrono()
    {
        tempsCourse += Time.deltaTime;

        int minutes = Mathf.FloorToInt(tempsCourse / 60f);
        int secondes = Mathf.FloorToInt(tempsCourse % 60f);
        int millisecondes = Mathf.FloorToInt((tempsCourse * 100f) % 100f);

        texteChrono.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, secondes, millisecondes);
    }

    // Met à jour la position du joueur parmi toutes les voitures en course
    void ActualiserPositions()
    {
        // Trie les voitures selon leur progression totale sur le circuit (du plus avancé au moins avancé)
        voitures = voitures.OrderByDescending(v => v.ObtenirProgressionTotale()).ToList();

        // Calcule et affiche la position du joueur
        int position = voitures.IndexOf(voitureJoueur) + 1;
        textePosition.text = "Position : " + position.ToString();
    }
}