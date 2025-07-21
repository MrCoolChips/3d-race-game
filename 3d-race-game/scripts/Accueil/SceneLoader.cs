//
/// Prenom : Rayane 
/// Nom: HOURI 
/// Date : 01/04/2025
/// Objectif du script : Ce script g�re la navigation entre les diff�rentes sc�nes du jeu MERY Speed
//          en r�ponse aux clics sur les boutons du menu. Il joue �galement un son de confirmation � chaque
//           clic pour am�liorer l�exp�rience utilisateur.


using UnityEngine;
using UnityEngine.SceneManagement;      // N�cessaire pour changer de sc�ne
using UnityEngine.UI;                  // Importation du module UI pour manipuler les �l�ments d'interface utilisateur.

public class SceneLoader : MonoBehaviour

{
    public AudioSource audioSource;     // R�f�rence � l'AudioSource qui jouera le son des boutons
    //public AudioClip buttonClickSound; // Son jou� lorsqu'un bouton est cliqu�
    public GameObject play;
    public GameObject menuPrincipale;

    // M�thode pour jouer le son du clic de bouton

    void Start()
    {
        if (PlayerPrefs.GetInt("StartBonus", 0)==0) {
            PlayerPrefs.SetInt("StartBonus", 1);
            PlayerPrefs.SetInt("Coin", 50000);
            PlayerPrefs.Save();
        }
    } 

    private void PlayButtonSound()

    {
        // V�rifie si l'AudioSource et le clip audio sont bien assign�s
        if (audioSource != null && audioSource != null)
        {
            audioSource.Play(); // Joue le son une seule fois
        }
    }

    // M�thode appel�e lorsque le bouton "Play" est cliqu�

    public void SelectionDeVoitures()
    {
        PlayButtonSound();                      // Joue le son du bouton
        play.gameObject.SetActive(true);
        GameObject.FindWithTag("MenuPrincipal").gameObject.SetActive(false); // Charge la sc�ne de jeu principale
    }

    // M�thode appel�e lorsque le bouton "Settings" est cliqu�
    public void LoadSettingsScene()
    {
        PlayButtonSound();                        // Joue le son du bouton
        SceneManager.LoadScene("SettingsScene"); // Charge la sc�ne des param�tres
    }

    // M�thode appel�e lorsque le bouton "Shop" est cliqu�

    public void LoadShopScene()
    {
        PlayButtonSound();                    // Joue le son du bouton
        SceneManager.LoadScene("ShopScene"); // Charge la sc�ne de la boutique
    }

    // M�thode appel�e lorsque le bouton "Stats" est cliqu�

    public void LoadStatsScene()
    {
        PlayButtonSound();                     // Joue le son du bouton
        SceneManager.LoadScene("StatsScene"); // Charge la sc�ne des statistiques
    }

    // M�thode pour quitter le jeu 

    public void QuitGame()
    {
        PlayButtonSound();              // Joue le son du bouton
        Debug.Log("Quitter le jeu !"); // Affiche un message dans la console (utile en mode d�veloppement)
        Application.Quit();           // Quitte l'application 
    }

    public void Button(GameObject menu) {
        PlayButtonSound();                      // Joue le son du bouton
        menu.SetActive(true);
        menuPrincipale.SetActive(false);
    }

    public void Back(GameObject menu) {
        PlayButtonSound();                      // Joue le son du bouton
        menuPrincipale.SetActive(true);
        menu.SetActive(false);
    }
}
