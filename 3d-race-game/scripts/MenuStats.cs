using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

class Cars {
    public String name;
    public int times;

    public Cars(String name, int times) {
        this.name = name;
        this.times = times;
    }
}

public class MenuStats : MonoBehaviour
{
    public TextMeshProUGUI tL;
    public TextMeshProUGUI tR;
    private String[] type = { "Typeless", "Taxi", "SUV", "Police", "Vintage", "Race"};
    private List<Cars> cars = new List<Cars>();
    private float best;
    private float best2;

    void Awake()
    {
       for (int i = 0; i < type.Length; i++) {
            cars.Add(new Cars(type[i], PlayerPrefs.GetInt("VoitureType_" + i, 0)));
        }
    }
    void OnEnable()
    {
        TextLeft();
        TextRight();
    }

    void TextLeft() {
        tL.text =  PlayerPrefs.GetInt("nombreDeJeux", 0) + " games played <br>";
        tL.text +=  "<color=#58AA32>" + PlayerPrefs.GetInt("victoire", 0) + " victory </color><br>";
        tL.text +=  "<color=#FF0000>" + PlayerPrefs.GetInt("defaite", 0) + " defeat </color><br>";
        tL.text +=   PlayerPrefs.GetInt("totalCoin", 0) + "<sprite=0>  earned <br><br>";
        best = PlayerPrefs.GetFloat("Le_Meilleur_LAP_0" , 0f);
        best2 = PlayerPrefs.GetFloat("Le_Meilleur_LAP_1" , 0f);
        tL.text += "fastest lap time on map 1 : " + Mathf.FloorToInt(best / 60f) + ":" + Mathf.FloorToInt(best % 60f) + ":"+ Mathf.FloorToInt((best * 100f) % 100f) + "<br>";
        tL.text += "fastest lap time on map 2 : " + Mathf.FloorToInt(best2 / 60f) + ":" + Mathf.FloorToInt(best2 % 60f) + ":"+ Mathf.FloorToInt((best2 * 100f) % 100f);
    }

    void TextRight() {
       for (int i = 0; i < type.Length; i++) {
            cars[i].times = PlayerPrefs.GetInt("VoitureType_" + i, 0);
        }
        tR.text = "";
        cars = cars.OrderByDescending(w=>w.times).ToList();

        foreach (Cars car in cars) {
            tR.text += car.name + ": " + car.times + "<br>";
        }
    }
}