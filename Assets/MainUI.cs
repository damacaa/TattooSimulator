using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUI : MonoBehaviour
{
    [SerializeField]
    GameObject _profilesButton;
    [SerializeField]
    GameObject _tattoosButton;

    private void Start()
    {
#if UNITY_WEBGL
        _profilesButton.SetActive(false);
#endif
    }
}
