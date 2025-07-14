//
// NOm: HOURI 
// Pr�nom :Rayane 
// Date :04/04/2025
// Objectif du script : il permet de jouer un son sp�cifique lorsqu'un utilisateur
//           clique sur un bouton dans l'interface du jeu, am�liorant ainsi l'interaction en ajoutant un retour sonore.
//           Il utilise un composant AudioSource pour diffuser le son du clic

using UnityEngine;
using UnityEngine.UI;  // Importation des fonctionnalit�s sp�cifiques � l'interface utilisateur, comme les boutons.


public class ButtonSound : MonoBehaviour // D�finition de la classe ButtonSound qui h�rite de MonoBehaviour.

{
    public AudioSource audioSource;    // R�f�rence � l'AudioSource qui joue le son. Il sera assign� dans l'Inspector.
    public AudioClip clickSound;      // R�f�rence au fichier audio pour le son du clic. Il sera assign� dans l'Inspector.
    public AudioClip quitSound;      // Son du clic pour le bouton "Quitter"

    void Start()                   // Fonction Start() qui est appel�e au d�but de l'ex�cution du script, juste avant le premier frame.
    {
        Button btn = GetComponent<Button>(); // R�cup�re le composant Button attach� � ce GameObject.

        if (btn != null)                   // Si le bouton a bien �t� trouv�...
        {
            btn.onClick.AddListener(PlayClickSound); // Ajoute un listener au bouton : chaque fois que le bouton est cliqu�, la m�thode PlayClickSound est appel�e.
           
        }
    }

    // Fonction pour jouer le son de clic
    void PlayQuitSound()
    {
        if (audioSource != null && quitSound != null)
        {
            audioSource.PlayOneShot(quitSound); // Joue le son de clic sp�cifique
        }
    }

    void PlayClickSound() // M�thode qui est appel�e chaque fois que le bouton est cliqu�.
    {
        if (audioSource != null && clickSound != null) // V�rifie que l'AudioSource et le fichier audio sont assign�s.
        {
            audioSource.PlayOneShot(clickSound);     // Joue le son du clic sans interrompre d'autres sons en cours.
        }
    }

}