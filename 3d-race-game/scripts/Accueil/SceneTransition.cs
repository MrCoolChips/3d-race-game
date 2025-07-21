//
// NOm: HOURI 
// Pr�nom :Rayane 
// Date :05/04/2025
// Objectif du script : Ce script est utile pour g�rer une musique de fond persistante et pour permettre
// une navigation simple entre les sc�nes du jeu .

using UnityEngine;
using UnityEngine.SceneManagement; // N�cessaire pour pouvoir changer de sc�ne

public class SceneTransition : MonoBehaviour

{
    public AudioSource music; // R�f�rence � une source audio (musique de fond ou effet sonore)

    void Start()
    {
        // Lorsque la sc�ne d�marre, on lance la musique si elle est assign�e et qu'elle n'est pas d�j� en train de jouer
        if (music != null && !music.isPlaying)
        {
            music.Play(); // Lance la musique au d�marrage de la sc�ne
        }
    }

    void Update()
    {
        // � chaque frame, on v�rifie si la touche "Espace" a �t� press�e
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Si oui, on charge une nouvelle sc�ne appel�e "MenuManager"
            SceneManager.LoadScene("MenuManager");
        }
    }
}




