using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class SalleExpositionDeVoitures : MonoBehaviour
{
    public float vitessevitesseDeRotation = 50f;

    public int vitesse;
    public int frein;
    public int acceleration;
    public int durabilite;
    public String nom;
    public int voitureIndex;

    public enum CarDriveType
    {
        FrontWheelDrive,
        RearWheelDrive,
        FourWheelDrive
    }

    public enum typeDeVoiture {
        Typeless,
        Taxi,
        SUV,
        Police,
        Vintage,
        Race
    }

    public CarDriveType driveType;
    public typeDeVoiture type;
    public bool dansMonGarage;
    public int prix;
    public bool retourne = false;

    public bool[][] ameliorations = new bool[][] {
        new bool[5],
        new bool[5],
        new bool[5],
        new bool[5],
        new bool[4]
    };

    /*
    0 = ameliorationsDuMoteur
    1 = ameliorationsDuFrein
    2 = ameliorationsDuTurbo
    3 = ameliorationsDuAcceleration
    4 = ameliorationsDuCouleur
    */

    void Start()
    {
        foreach (bool[] bol in ameliorations) {
            bol[0] = true;
        }
    }

    void Update()
    {
        if (!retourne) {
            transform.Rotate(Vector3.up, vitessevitesseDeRotation * Time.deltaTime, Space.World);
        }
    }
}
