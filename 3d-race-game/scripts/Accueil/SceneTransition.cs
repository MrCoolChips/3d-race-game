//
// NOm: HOURI 
// Prénom :Rayane 
// Date :05/04/2025
// Objectif du script : Ce script est utile pour gérer une musique de fond persistante et pour permettre
// une navigation simple entre les scènes du jeu .

using UnityEngine;
using UnityEngine.SceneManagement; // Nécessaire pour pouvoir changer de scène

public class SceneTransition : MonoBehaviour

{
    public AudioSource music; // Référence à une source audio (musique de fond ou effet sonore)

    void Start()
    {
        // Lorsque la scène démarre, on lance la musique si elle est assignée et qu'elle n'est pas déjà en train de jouer
        if (music != null && !music.isPlaying)
        {
            music.Play(); // Lance la musique au démarrage de la scène
        }
    }

    void Update()
    {
        // À chaque frame, on vérifie si la touche "Espace" a été pressée
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Si oui, on charge une nouvelle scène appelée "MenuManager"
            SceneManager.LoadScene("MenuManager");
        }
    }
}




