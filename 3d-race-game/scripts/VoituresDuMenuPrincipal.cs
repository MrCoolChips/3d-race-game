using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoituresDuMenuPrincipal : MonoBehaviour
{
    public GameObject[] voitures;
    public Material[] couleurs;

    void OnEnable()
    {
        foreach (var voiture in voitures) {
            voiture.SetActive(false);
        }
        int index = PlayerPrefs.GetInt("voitureSelectionne", 0);
        voitures[index].GetComponentInChildren<MeshRenderer>().material = couleurs[PlayerPrefs.GetInt(voitures[index].name + "Couleur", 0)];
        voitures[index].SetActive(true); 
    }
}
