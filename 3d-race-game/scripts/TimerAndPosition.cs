using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class Voitures {
    public GameObject voiture;
    public int position;

    public Voitures(GameObject obj, int pos) {
        voiture = obj;
        position = pos;
    }

}

public class TimerAndPosition : MonoBehaviour
{
    private float raceTime = 0f;  // Temps de course
    public TextMeshProUGUI timerText;        // Référence au composant Text pour afficher le chrono
    public List<Voitures> cars = new List<Voitures>(); // Liste de toutes les voitures en course
    public TextMeshProUGUI classement; // Référence au composant Text pour afficher la position du joueur
    public TextMeshProUGUI classementAvecChiffre; 
    public TextMeshProUGUI suffixe; 
    public TextMeshProUGUI LAP; 
    int nombreDeJoueur;
    int nombreDeLAP;
    public List<GameObject> finDeCourse = new List<GameObject>();
    public TextMeshProUGUI classementFinDeJeu;
    void Start() {
        nombreDeJoueur = PlayerPrefs.GetInt("nombreDeJoueur");
        nombreDeLAP = PlayerPrefs.GetInt("nombreDeLAP");
        LAP.text = "LAP 1" + "/" + nombreDeLAP;
    }

    void Update()
    {
        // Mettre à jour le chrono
        raceTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(raceTime / 60f);
        int seconds = Mathf.FloorToInt(raceTime % 60f);
        int milliseconds = Mathf.FloorToInt((raceTime * 100f) % 100f);

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }
    public void RecevoirVoiture(GameObject obj, int pos) {
        cars.Add(new Voitures(obj, pos));

        if (cars.Count == nombreDeJoueur) {
            VerifieClassement();
        }
    }

    public void RecevoirLAP(GameObject obj) {
        if (obj.name == "YOU") {
            obj.GetComponent<ClassementDuCourse>().positionDansLaCourse = finDeCourse.Count() + 1;
            finDeCourse.Add(obj);
        } else {
            finDeCourse.Add(obj);
        }
    }

    public void MettreAJourLeClassement(GameObject obj, int pos) {
        for (int i = 0; i < cars.Count; i++) {
            if (cars[i].voiture == obj) {
                cars[i].position = pos;
            }
        }
        VerifieClassement();
    }

    public void VerifieClassement() {

        cars = cars.OrderByDescending(w=>w.position).ToList();
        classement.text = "";
        for (int i = 0; i < cars.Count; i++) {
            if (cars[i].voiture.name == "YOU") {
                    classement.text += "<color=#FF0000>" + (i+1).ToString() + "   " + cars[i].voiture.name + "<br></color>";
                    cars[i].voiture.GetComponent<ClassementDuCourse>().positionDansLaCourse = i;
                    classementAvecChiffre.text = (i+1).ToString();
                    if (i+1 == 1) {
                        suffixe.text = "st";    
                    } else if (i+1 == 2) {
                        suffixe.text = "nd";  
                    } else if (i+1 == 3) {
                        suffixe.text = "rd";  
                    } else {
                        suffixe.text = "th"; 
                    }
            } else {
                    classement.text += i+1 + "   " + cars[i].voiture.name + "<br>";
                    cars[i].voiture.GetComponent<ClassementDuCourse>().positionDansLaCourse = i;
            }
        }
    }

    public void VerifieLAP(int lap) {
        LAP.text = "LAP " + lap + "/" + nombreDeLAP;
    }

    public void ClassementPourFinDeJeu() {
        classementFinDeJeu.text = "";
        int secondesTotales = 0;
        int millisecondesTotales = 0;
        cars = cars.OrderByDescending(w=>w.position).ToList();
        for (int i = 0; i < cars.Count; i++) {
            ClassementDuCourse voiture = cars[i].voiture.GetComponentInChildren<ClassementDuCourse>();
            if (i < finDeCourse.Count) {
                if ( voiture.gameObject.name == "YOU") {
                    classementFinDeJeu.text += "<color=#FF0000>" + (i+1).ToString() + "  " + voiture.gameObject.name + "   " + voiture.minutes.ToString("00") + ":" + voiture.seconds.ToString("00") + ":" + voiture.milliseconds.ToString("000") + "</color><br>";
                } else {
                    classementFinDeJeu.text += (i+1).ToString() + "  " + voiture.gameObject.name + "   " + voiture.minutes.ToString("00") + ":" + voiture.seconds.ToString("00") + ":" + voiture.milliseconds.ToString("000") + "<br>";
                }
            } else {

                secondesTotales +=  Random.Range(1, 30);
                millisecondesTotales += Random.Range(100, 300);

                voiture.milliseconds += millisecondesTotales;
                voiture.seconds += secondesTotales;
                while (voiture.milliseconds >= 1000)
                {
                    voiture.seconds += millisecondesTotales / 1000;
                    voiture.milliseconds = millisecondesTotales % 1000;
                }

                while (voiture.seconds >= 60)
                {
                    voiture.minutes += voiture.seconds / 60;
                    voiture.seconds = voiture.seconds % 60;
                }

                classementFinDeJeu.text += (i + 1).ToString() + "  " + voiture.gameObject.name + "   " + voiture.minutes.ToString("00") + ":" + voiture.seconds.ToString("00") + ":" + voiture.milliseconds.ToString("000") + "<br>";
            }
        }
    }

}
