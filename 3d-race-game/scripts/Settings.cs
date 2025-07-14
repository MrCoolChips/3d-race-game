using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Slider menuSlider;
    public GameObject musicManager;
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown resolutionDropdown;
    private List<Vector2Int> resolutions = new List<Vector2Int>()
    {
        new Vector2Int(2560, 1440),
        new Vector2Int(1920, 1080),
        new Vector2Int(1280, 720)
    };
    public Toggle fullscreenToggle;
    private List<string> options = new List<string>();
    public GameObject resolutionPanel;

    void OnEnable()
    {
        InitializeResolutionSettings();
        menuSlider.value = PlayerPrefs.GetFloat("SonDuMenu", 1);
        qualityDropdown.value = PlayerPrefs.GetInt("Qualite", 4);
        bool savedFullscreen = PlayerPrefs.GetInt("isFullscreen", 1) == 1;
        fullscreenToggle.isOn = savedFullscreen;
        QualitySettings.SetQualityLevel(qualityDropdown.value);
        int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 1);
        ApplyResolution(savedResolutionIndex, savedFullscreen);
        if (savedFullscreen)
        {
            resolutionPanel.SetActive(false);
        }
        else
        {
            resolutionPanel.SetActive(true);
        }
    }

    public void AdjustTheVolumeWithSlider()
    {
        PlayerPrefs.SetFloat("SonDuMenu", menuSlider.value);
        PlayerPrefs.Save();
        if (musicManager != null) {
            musicManager.GetComponent<MusicManager>().AdjustTheVolume();
        }
    }

    public void AdjustTheQuality()
    {
        PlayerPrefs.SetInt("Qualite", qualityDropdown.value);
        PlayerPrefs.Save();
        QualitySettings.SetQualityLevel(qualityDropdown.value);
    }

    void InitializeResolutionSettings()
    {
        resolutionDropdown.ClearOptions();
        options.Clear();
        for (int i = 0; i < resolutions.Count; i++)
        {
            string option = resolutions[i].x + " x " + resolutions[i].y;
            options.Add(option);
        }
        resolutionDropdown.AddOptions(options);
        int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 1);
        resolutionDropdown.value = savedResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionDropdownChanged(resolutionDropdown.value); });
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);
    }

    void OnResolutionDropdownChanged(int index)
    {
        ApplyResolution(index, fullscreenToggle.isOn);
    }

    void OnFullscreenToggleChanged(bool isFullscreen)
    {
        PlayerPrefs.SetInt("isFullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
        ApplyResolution(resolutionDropdown.value, isFullscreen);
        if (isFullscreen)
        {
            resolutionPanel.SetActive(false);
            int index = resolutions.FindIndex(res => res.x == 1920 && res.y == 1080);
            if (index != -1)
            {
                resolutionDropdown.value = index;
                ApplyResolution(index, isFullscreen);
            }
        }
        else
        {
            resolutionPanel.SetActive(true);
        }
    }

    private void ApplyResolution(int index, bool isFullscreen)
    {
        int width = resolutions[index].x;
        int height = resolutions[index].y;
        PlayerPrefs.SetInt("width", width);
        PlayerPrefs.SetInt("height", height);
        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save();
        if (isFullscreen)
        {
            Screen.SetResolution(width, height, FullScreenMode.ExclusiveFullScreen);
        }
        else
        {
            Screen.SetResolution(width, height, FullScreenMode.Windowed);
        }
    }
}
