using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager
{
    #region SINGLETON
    static FileManager _instance;
    public static FileManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new FileManager();

            return _instance;
        }
    }
    #endregion

    #region VARIABLES
    string _root;
    string _currentProfileFolder;

    public string CurrentProfileDataPath
    {
        get { return $"{_currentProfileFolder}data.txt"; }
    }

    public string ImageFolderPath
    {
        get
        {
            string path = $"{_currentProfileFolder}/images";
            Helpers.SafeCreateDirectory(path);
            return path;
        }
    }

    string SettingsPath
    {
        get { return $"{_root}/settings.txt"; }
    }
    #endregion

    // Private constructor forces to use singleton
    private FileManager()
    {

#if UNITY_STANDALONE || UNITY_EDITOR
        _root = Application.dataPath;
#else
        _root =  Application.persistentDataPath;
#endif

        Helpers.SafeCreateDirectory($"{_root}/Saves");
    }

    public string[] GetAllAvailableProfiles()
    {
        string[] names = Directory.GetDirectories($"{_root}/Saves/", "*", SearchOption.TopDirectoryOnly);
        int offset = ($"{_root}/Saves/").Length;
        for (int i = 0; i < names.Length; i++)
        {
            names[i] = names[i].Substring(offset);
        }

        return names;
    }

    public void SetCurrentProfile(string profile)
    {
        _currentProfileFolder = $"{_root}/Saves/{profile}/";
    }

    public void DeleteCurrentProfile()
    {
        if (Directory.Exists(_currentProfileFolder)) { Directory.Delete(_currentProfileFolder, true); }
    }

    public void SaveData(string data)
    {
        SaveFile(data, CurrentProfileDataPath);
    }

    public string LoadData()
    {
        return LoadFile(CurrentProfileDataPath);
    }

    public void SaveSettings(string data)
    {
        SaveFile(data, SettingsPath);
    }

    public string LoadSettings()
    {
        return LoadFile(SettingsPath);
    }

    void SaveFile(string data, string path)
    {
        try
        {
            Helpers.SafeCreateDirectory(Directory.GetParent(path).ToString());

#if UNITY_STANDALONE || UNITY_EDITOR

            File.WriteAllText(path, data);

#elif UNITY_ANDROID

            StreamWriter Writer = new StreamWriter(path);
            Writer.Write(data);
            Writer.Flush();
            Writer.Close();

#endif

        }
        catch (Exception)
        {
            throw;
        }
    }

    string LoadFile(string _path)
    {
        string data;

        Debug.Log($"Loading: {_path}");

        if (!File.Exists(_path))
        {
            Debug.Log($"File not found: {_path}");
            return null;
        }

        data = File.ReadAllText(_path);

        return data;
    }
}
