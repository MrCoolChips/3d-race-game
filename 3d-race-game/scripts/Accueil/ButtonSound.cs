//
// NOm: HOURI 
// Prénom :Rayane 
// Date :04/04/2025
// Objectif du script : il permet de jouer un son spécifique lorsqu'un utilisateur
//           clique sur un bouton dans l'interface du jeu, améliorant ainsi l'interaction en ajoutant un retour sonore.
//           Il utilise un composant AudioSource pour diffuser le son du clic

using UnityEngine;
using UnityEngine.UI;  // Importation des fonctionnalités spécifiques à l'interface utilisateur, comme les boutons.


public class ButtonSound : MonoBehaviour // Définition de la classe ButtonSound qui hérite de MonoBehaviour.

{
    public AudioSource audioSource;    // Référence à l'AudioSource qui joue le son. Il sera assigné dans l'Inspector.
    public AudioClip clickSound;      // Référence au fichier audio pour le son du clic. Il sera assigné dans l'Inspector.
    public AudioClip quitSound;      // Son du clic pour le bouton "Quitter"

    void Start()                   // Fonction Start() qui est appelée au début de l'exécution du script, juste avant le premier frame.
    {
        Button btn = GetComponent<Button>(); // Récupère le composant Button attaché à ce GameObject.

        if (btn != null)                   // Si le bouton a bien été trouvé...
        {
            btn.onClick.AddListener(PlayClickSound); // Ajoute un listener au bouton : chaque fois que le bouton est cliqué, la méthode PlayClickSound est appelée.
           
        }
    }

    // Fonction pour jouer le son de clic
    void PlayQuitSound()
    {
        if (audioSource != null && quitSound != null)
        {
            audioSource.PlayOneShot(quitSound); // Joue le son de clic spécifique
        }
    }

    void PlayClickSound() // Méthode qui est appelée chaque fois que le bouton est cliqué.
    {
        if (audioSource != null && clickSound != null) // Vérifie que l'AudioSource et le fichier audio sont assignés.
        {
            audioSource.PlayOneShot(clickSound);     // Joue le son du clic sans interrompre d'autres sons en cours.
        }
    }

}