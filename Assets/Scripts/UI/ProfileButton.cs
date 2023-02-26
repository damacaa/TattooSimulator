using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
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
            print($"Current profile: {profileName}");
            ProfileManager.instance.SetProfile(profileName);
            ProfileManager.instance.LoadData();
            UIManager.instance.HideProfiles();
            UIManager.instance.ShowUI();
        }
    }
}
