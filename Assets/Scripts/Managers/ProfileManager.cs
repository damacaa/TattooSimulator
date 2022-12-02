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

    public FileManager fileManager;

    [SerializeField]
    string profile = "data";
    Settings settings;

    private void Start()
    {
#if !UNITY_WEBGL
        //fileManager.Initialize();
        fileManager = new FileManager();

        string[] availableProfiles = fileManager.GetAllAvailableProfiles();
        UIManager.instance.showAvailableProfiles(availableProfiles);

        string settingsData = fileManager.LoadSettings();
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

        LoadData();
#endif
    }

    public void SetProfile(string s)
    {
        profile = s;
        UIManager.instance.UpdateProfile(profile);
        fileManager.SetProfile(profile);
        if (settings != null)
        {
            settings.lastProfile = profile;
            string settingsData = JsonUtility.ToJson(settings);
            fileManager.SaveSettings(settingsData);
        }
    }

    List<TattooInfo> unableToLoadTattoos = new List<TattooInfo>();
    public void LoadData()
    {
#if !UNITY_WEBGL
        unableToLoadTattoos.Clear();
        TattooManager.instance.Reset();

        DesignManager.instance.LoadDesigns();

        string data = fileManager.Load();
        if (data == null)
            return;

        TattoList tattoList = JsonUtility.FromJson<TattoList>(data);
        foreach (TattooInfo t in tattoList.tattoos)
        {
            Texture tex;
            if (!DesignManager.instance.textures.TryGetValue(t.texture, out tex))
            {
                unableToLoadTattoos.Add(t);
                print("Unable to load " + t.texture);
                continue;
            }

            TattooManager.instance.SpawnTattoo(t.position, t.euler, tex, t.size);
        }
        Save();
#endif
    }

    public void Save()
    {
#if !UNITY_WEBGL
        print("Saving: " + fileManager.DataPath);

        List<TattooInfo> tattooInfos = new List<TattooInfo>();
        foreach (SmartTattoo smartTattoo in TattooManager.instance.spawnedTattoos)
        {
            tattooInfos.Add(smartTattoo.GetInfo());
        }

        tattooInfos.AddRange(unableToLoadTattoos);

        TattoList tattooList = new TattoList();
        tattooList.tattoos = tattooInfos.ToArray();

        string data = JsonUtility.ToJson(tattooList);

        fileManager.Save(data);

        string settingsData = JsonUtility.ToJson(settings);
        fileManager.SaveSettings(settingsData);
#endif
    }

    public void DeleteProfile()
    {
        UIManager.instance.ShowDeleteConfirmatioWindow();
    }

    public void ConfirmDelete()
    {
        fileManager.DeleteCurrentProfile();
        SetProfile("Default");
        LoadData();
        Save();
        UIManager.instance.HideDeleteConfirmatioWindow();
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
