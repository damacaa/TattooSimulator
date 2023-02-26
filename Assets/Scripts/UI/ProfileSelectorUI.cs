using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class ProfileSelectorUI : MonoBehaviour
    {
        [SerializeField]
        GameObject profileButtonPrefab;
        [SerializeField]
        Transform profileList;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        internal void showProfiles(string[] availableProfiles)
        {
            foreach (string profile in availableProfiles)
            {
                GameObject newProfileButton = Instantiate(profileButtonPrefab, profileList);
                newProfileButton.GetComponent<ProfileButton>().SetProfile(profile);
            }
        }
    }
}