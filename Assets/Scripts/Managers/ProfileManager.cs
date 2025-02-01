using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager instance;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField]
    string profile = "data";
    Settings settings;

    private void Start()
    {
#if !UNITY_WEBGL

        string[] availableProfiles = FileManager.Instance.GetAllAvailableProfiles();
        UI.UIManager.instance.ShowProfiles(availableProfiles);

        string settingsData = FileManager.Instance.LoadSettings();
        if (settingsData != null && settingsData != "")
        {
            settings = JsonUtility.FromJson<Settings>(settingsData);
            if (settings.lastProfile != null && settings.lastProfile != "")
                profile = settings.lastProfile;
        }
        else
        {
            settings = new Settings();
        }

        SetProfile(profile);

#endif
        LoadData();

    }

    public void SetProfile(string s)
    {
        profile = s;
        UI.UIManager.instance.UpdateProfile(profile);
        FileManager.Instance.SetCurrentProfile(profile);
        if (settings != null)
        {
            settings.lastProfile = profile;
            string settingsData = JsonUtility.ToJson(settings);
            FileManager.Instance.SaveSettings(settingsData);
        }
    }

    List<TattooInfo> unableToLoadTattoos = new List<TattooInfo>();
    public void LoadData()
    {
#if !UNITY_WEBGL
        unableToLoadTattoos.Clear();
        TattooSpawner.Instance.Reset();

        DesignManager.Instance.LoadDesigns();

        string data = FileManager.Instance.LoadData();
        if (data == null)
            return;

        TattoList tattoList = JsonUtility.FromJson<TattoList>(data);
        foreach (TattooInfo t in tattoList.tattoos)
        {
            Texture tex;
            if (!DesignManager.Instance.textures.TryGetValue(t.texture, out tex))
            {
                unableToLoadTattoos.Add(t);
                print("Unable to load " + t.texture);
                continue;
            }

            TattooSpawner.Instance.SpawnTattoo(t.position, t.euler, tex, t.size);
        }
        Save();
#else
        DesignManager.Instance.LoadDesigns();
#endif
    }

    public void Save()
    {
#if !UNITY_WEBGL
        print("Saving: " + FileManager.Instance.CurrentProfileDataPath);

        List<TattooInfo> tattooInfos = new List<TattooInfo>();
        foreach (SmartTattoo smartTattoo in TattooSpawner.Instance.spawnedTattoos)
        {
            tattooInfos.Add(smartTattoo.GetInfo());
        }

        tattooInfos.AddRange(unableToLoadTattoos);

        TattoList tattooList = new TattoList();
        tattooList.tattoos = tattooInfos.ToArray();

        string data = JsonUtility.ToJson(tattooList);

        FileManager.Instance.SaveData(data);

        string settingsData = JsonUtility.ToJson(settings);
        FileManager.Instance.SaveSettings(settingsData);
#endif
    }

    public void DeleteProfile()
    {
        UI.UIManager.instance.ShowDeleteConfirmatioWindow();
    }

    public void ConfirmDelete()
    {
        FileManager.Instance.DeleteCurrentProfile();
        SetProfile("Default");
        LoadData();
        Save();
        UI.UIManager.instance.HideDeleteConfirmatioWindow();
    }



    [System.Serializable]
    class TattoList
    {
        public TattooInfo[] tattoos;
    }

    [System.Serializable]
    class Settings
    {
        public string lastProfile;
    }

}
