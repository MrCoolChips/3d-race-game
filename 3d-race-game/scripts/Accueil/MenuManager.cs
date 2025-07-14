/// 
/// Prenom : Rayane 
/// Nom: HOURI 
/// Date : 23/03/2025
/// Objectif du script : Le script MenuManager gère l'affichage des écrans du menu,
/// en permettant la transition de l'écran de démarrage au menu principal lorsqu'une touche est pressée.
/// Il ajoute aussi une musique spécifique à chaque écran.

using UnityEngine.SceneManagement;  // Nécessaire pour changer de scène
using UnityEngine;
using UnityEngine.UI;   /// Importation du module UI pour manipuler les éléments d'interface utilisateur.

public class MenuManager : MonoBehaviour

{
    /// Références aux images UI pour gérer l'affichage des écrans du menu.
    public Image startScreen;            /// Image représentant l'écran de démarrage.
    public Image mainMenu;              /// Image représentant le menu principal.

    /// Références aux fichiers audio pour chaque écran.
   
   // public AudioClip startScreenMusic;  /// Musique pour l'écran de démarrage.
    public AudioClip mainMenuMusic;     /// Musique pour le menu principal.

    private AudioSource audioSource;   ///  Composant AudioSource qui jouera la musique.

    void Start()
    {
        ///  Initialisation des écrans :
       
        /// Active l'écran de démarrage et désactive le menu principal au lancement du jeu.
        //startScreen.gameObject.SetActive(true);
        mainMenu.gameObject.SetActive(false);

        ///  Récupération du composant AudioSource attaché à ce GameObject.
        audioSource = GetComponent<AudioSource>();

        /// Jouer la musique de l'écran de démarrage.
        //PlayMusic(startScreenMusic);
    }

    void Update()
    {
        ///  Vérifie si le joueur appuie sur la touche ESPACE.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowMainMenu();  /// Appelle la fonction pour afficher le menu principal.
        }
        // Vérifie si l'utilisateur appuie sur la touche "P" (Utilisé pour gérer le bug des bouttons que j'ai rencontré ) 
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Change la scène en fonction de son nom
            SceneManager.LoadScene("SampleScene");  // On active la scene "SampleScene"
        }
    }

    void ShowMainMenu()
    {
        ///  Transition vers le menu principal :
        /// Désactive l'écran de démarrage et active le menu principal.
        
        //startScreen.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);

        /// Changer la musique pour celle du menu principal.
        PlayMusic(mainMenuMusic);
    }

    void PlayMusic(AudioClip music)
    {
        ///  Vérifie que l’AudioSource et la musique existent avant de jouer.
        
        if (audioSource != null && music != null)
        {
            audioSource.clip = music;        /// Assigne le clip audio à l'AudioSource.
            audioSource.Play();             /// Joue la musique.
        }
    }
 
}


