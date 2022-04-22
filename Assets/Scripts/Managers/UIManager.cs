using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField]
    GameObject UI;
    [SerializeField]
    GameObject settings;
    [SerializeField]
    GameObject selector;

    [SerializeField]
    Slider angleSlider;
    [SerializeField]
    Slider sizeSlider;
    [SerializeField]
    TMP_InputField profileSelector;
    [SerializeField]
    GameObject deleteConfirmationWindow;

    void Start()
    {
        UI.SetActive(true);
        settings.SetActive(false);
        selector.SetActive(false);
        deleteConfirmationWindow.SetActive(false);
#if UNITY_WEBGL
        profileSelector.gameObject.SetActive(false);
#endif
    }

    public void ShowSettings()
    {
        settings.SetActive(true);
    }

    public void HideSettings()
    {
        settings.SetActive(false);
    }

    public void SetSettings(float angle, float size)
    {
        angleSlider.value = angle;
        sizeSlider.value = size;
    }

    public void UpdateProfile(string s)
    {
        profileSelector.text = s;
    }

    public void ShowDeleteConfirmatioWindow()
    {
        deleteConfirmationWindow.SetActive(true);
    }

    public void HideDeleteConfirmatioWindow()
    {
        deleteConfirmationWindow.SetActive(false);
    }
}
