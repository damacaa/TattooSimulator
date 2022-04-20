using System;
using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
        UI.SetActive(true);
        settings.SetActive(false);
        selector.SetActive(false);
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
}
