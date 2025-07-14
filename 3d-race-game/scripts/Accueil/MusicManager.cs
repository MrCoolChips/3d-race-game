using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource[] audioSources;

    private AudioSource currentSource;

    void Start()
    {
        PlayRandomMusic();
    }

    void Update()
    {
        if (currentSource != null && !currentSource.isPlaying)
        {
            PlayRandomMusic();
        }
    }

    void PlayRandomMusic()
    {
        if (audioSources.Length == 0) return;

        if (currentSource != null)
        {
            currentSource.Stop();
        }

        AudioSource nextSource;
        do
        {
            nextSource = audioSources[Random.Range(0, audioSources.Length)];
        } while (nextSource == currentSource && audioSources.Length > 1);

        currentSource = nextSource;
        currentSource.Play();
        AdjustTheVolume();
    }

    public void AdjustTheVolume() {
        if (currentSource != null) {
            if (PlayerPrefs.HasKey("width")) {
                currentSource.volume = PlayerPrefs.GetFloat("SonDuMenu");
                QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Qualite"));
                if (PlayerPrefs.GetInt("fullScreen") == 0) {
                    Screen.SetResolution(PlayerPrefs.GetInt("width"), PlayerPrefs.GetInt("height"), false);
                }
            } else {
                PlayerPrefs.SetFloat("SonDuMenu", 1);
                PlayerPrefs.SetInt("Qualite", 4);
                PlayerPrefs.SetInt("width", 1920);
                PlayerPrefs.SetInt("height", 1080);
                PlayerPrefs.SetInt("fullScreen", 1);
                PlayerPrefs.Save();

            } 
        }       
    }
}

