using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class KeySelector : MonoBehaviour
{
    public TMP_Dropdown lightsDropdown;
    public TMP_Dropdown hornDropdown;
    public TMP_Dropdown boostDropdown;
    public TMP_Dropdown rewindDropdown;

    public static KeyCode LightsKey = KeyCode.E;
    public static KeyCode HornKey = KeyCode.H;
    public static KeyCode BoostKey = KeyCode.LeftShift;
    public static KeyCode BrakeKey = KeyCode.Space;
    public static KeyCode RewindKey = KeyCode.R;

    private void Awake()
    {
        LoadAllKeys();
    }

    private void Start()
    {
        SetupDropdown(lightsDropdown, "LightsKey", LightsKey, (k) => LightsKey = k);
        SetupDropdown(hornDropdown, "HornKey", HornKey, (k) => HornKey = k);
        SetupDropdown(boostDropdown, "BoostKey", BoostKey, (k) => BoostKey = k);
        SetupDropdown(rewindDropdown, "RewindKey", RewindKey, (k) => RewindKey = k);
    }

    private void SetupDropdown(TMP_Dropdown dropdown, string prefKey, KeyCode defaultKey, Action<KeyCode> setter)
    {
        dropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new();
        foreach (KeyCode k in Enum.GetValues(typeof(KeyCode)))
            options.Add(new TMP_Dropdown.OptionData(k.ToString()));

        dropdown.AddOptions(options);

        KeyCode savedKey = LoadKey(prefKey, defaultKey);
        setter(savedKey);

        int index = dropdown.options.FindIndex(opt => opt.text == savedKey.ToString());
        if (index >= 0) dropdown.value = index;

        dropdown.onValueChanged.AddListener(i =>
        {
            KeyCode newKey = (KeyCode)Enum.Parse(typeof(KeyCode), dropdown.options[i].text);
            setter(newKey);
            PlayerPrefs.SetString(prefKey, newKey.ToString());
            PlayerPrefs.Save();
        });
    }

    private void LoadAllKeys()
    {
        LightsKey = LoadKey("LightsKey", KeyCode.E);
        HornKey = LoadKey("HornKey", KeyCode.H);
        BoostKey = LoadKey("BoostKey", KeyCode.LeftShift);
        BrakeKey = LoadKey("BrakeKey", KeyCode.Space);
        RewindKey = LoadKey("RewindKey", KeyCode.R);
    }

    private KeyCode LoadKey(string prefKey, KeyCode defaultKey)
    {
        if (PlayerPrefs.HasKey(prefKey))
        {
            string str = PlayerPrefs.GetString(prefKey);
            if (Enum.TryParse(str, out KeyCode key))
                return key;
        }
        return defaultKey;
    }
}
