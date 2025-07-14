//
// NOm: HOURI 
// Pr�nom :Rayane 
// Date :03/04/2025
// Objectif du script :il permet de g�rer un menu de pause dans un jeu Unity.
//            Il met le jeu en pause avec la touche "Entr�e", arr�te le temps et les adversaires, et affiche un menu permettant
//               de reprendre la partie ou de revenir au menu principal.



using UnityEngine;                 // Importation des fonctionnalit�s de base de Unity
using UnityEngine.SceneManagement; // N�cessaire pour g�rer le changement de sc�ne

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // R�f�rence � l'UI du menu de pause
    private bool isPaused = false; // Bool�en qui indique si le jeu est en pause
    public AudioSource moteurAudio; // R�f�rence � l'AudioSource pour le son du moteur
    public GameObject[] images;
    public GameObject settings;

    void Update()
    {
        // V�rifie si la touche "Entr�e" (Return) est press�e

        if (Input.GetKeyDown(KeyCode.Return))
        {
            // Si le jeu est d�j� en pause, on le reprend ; sinon, on le met en pause
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    //  Fonction pour reprendre le jeu apr�s une pause
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false); // Cache l'UI du menu de pause
        Time.timeScale = 1f;         // Remet le temps du jeu � la normale
        isPaused = false;           // Met � jour l'�tat de pause

        // V�rifie si l'AudioSource du moteur est assign�e avant de reprendre le son

        if (moteurAudio != null)
        {
            moteurAudio.UnPause(); // Reprend le son du moteur
        }
    }

    //  Fonction pour mettre le jeu en pause
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true); // Affiche l'UI du menu de pause
        Time.timeScale = 0f;        // Met le jeu en pause (arr�te le temps)
        isPaused = true;           // Met � jour l'�tat de pause

        // V�rifie si l'AudioSource du moteur est assign�e avant de stopper le son

        if (moteurAudio != null)
        {
            moteurAudio.Pause(); // Met le son du moteur en pause
        }
    }

    //  Fonction pour quitter la partie et retourner au menu principal
    public void LoadMenuManager()
    {
        Time.timeScale = 1f;                    // Remet le temps � la normale avant de quitter la sc�ne
        SceneManager.LoadScene("MenuManager"); // Charge la sc�ne du menu principal
    }

    public void Settings() {
        foreach (var item in images) {
            item.SetActive(false);
        }
        settings.SetActive(true);
    }

    public void Back() {
        foreach (var item in images) {
            item.SetActive(true);
        }
        settings.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();           // Quitte l'application 
    }
}
