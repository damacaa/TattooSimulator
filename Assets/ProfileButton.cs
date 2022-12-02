using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfileButton : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI buttonText;
    [SerializeField]
    string profileName;
    public void SetProfile(string profile)
    {
        profileName = profile;
        buttonText.text = profileName;
    }

    public void SelectProfile()
    {
        print(profileName);
        ProfileManager.instance.SetProfile(profileName);
        ProfileManager.instance.LoadData();
        UIManager.instance.hideAvailableProfiles();
    }
}
