using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
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
        GameObject profileSelectorWindow;


        [SerializeField]
        TMP_InputField profileSelector;
        [SerializeField]
        GameObject deleteConfirmationWindow;

        internal void ShowProfiles(string[] availableProfiles)
        {
            profileSelectorWindow.SetActive(true);
            ProfileSelectorUI profileSelector = profileSelectorWindow.GetComponent<ProfileSelectorUI>();
            profileSelector.showProfiles(availableProfiles);
        }

        internal void ShowUI()
        {
            CloseTattooSelector();
        }

        internal void HideProfiles()
        {
            profileSelectorWindow.SetActive(false);
        }

        void Start()
        {
            UI.SetActive(false);

            settings.SetActive(false);
            selector.SetActive(false);
            deleteConfirmationWindow.SetActive(false);

#if UNITY_WEBGL
            UI.SetActive(true);
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

        public void CloseTattooSelector()
        {
            UI.SetActive(true);
            //settings.SetActive(false);
            selector.SetActive(false);
        }
    }
}
