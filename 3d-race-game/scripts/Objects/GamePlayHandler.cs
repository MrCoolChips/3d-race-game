using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayHandler : MonoBehaviour
{
    public static GamePlayHandler Instance;
    public Image healthFillBar;

    public TextMeshProUGUI coinsTxt;
    public float health=100f;
    public float maxHealth;
    public float slipTime;
    public float speedCamera;
    public AudioSource[] audios;

    private void Awake()
    {
        Instance = this;
        health = maxHealth;
        UpdateHealth();
        UpdateCoins();
    }
    public void UpdateCoins()
    {
        coinsTxt.text=PlayerPrefs.GetInt("Coin",0).ToString();
    }
    public void UpdateHealth()
    {
        healthFillBar.fillAmount=health/maxHealth;
    }
    public void GetDamage(float damage, GameObject obj)
    {
        health-=damage;
        if (damage < 0) {
            audios[2].Play();    
        } else {
            audios[1].Play();
        }

        if (health > maxHealth) 
        { 
            health = maxHealth; 
        }
        else if(health <= 0) 
        { 
            health = maxHealth;
            GameObject.Find("YOU").GetComponent<ClassementDuCourse>().Reapparition(); 

        }
        UpdateHealth();
        StartCoroutine(RespawnObject(obj));


    }
    public void AddCoins(int val, GameObject obj)
    {
        audios[0].Play();
        PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin", 0)+val);
        PlayerPrefs.Save();
        UpdateCoins();
        StartCoroutine(RespawnObject(obj));
    }

    IEnumerator RespawnObject(GameObject obj) {
        yield return new WaitForSeconds(15f);
        obj.SetActive(true);

    }
}
