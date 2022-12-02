using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager
{
    public string root
    {
        get
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            return Application.dataPath;
#else
            return Application.persistentDataPath;
#endif
        }
    }
    public string currentProfileFolder;

    public string DataPath
    {
        get { return currentProfileFolder + "data.txt"; }
    }

    public string ImageFolderPath
    {
        get
        {
            string path = currentProfileFolder + "/images";
            Helpers.SafeCreateDirectory(path);
            return path;
        }
    }

    public string SettingsPath
    {
        get { return root + "/settings.txt"; }
    }

    public FileManager()
    {
        Initialize();
    }

    public void Initialize()
    {
        Helpers.SafeCreateDirectory(root + "/Saves");
    }

    public void SetProfile(string profile)
    {
        currentProfileFolder = root + "/Saves/" + profile + "/";
    }

    public string[] GetAllAvailableProfiles()
    {
        string[] names = Directory.GetDirectories(root + "/Saves/", "*", SearchOption.TopDirectoryOnly);
        int offset = (root + "/Saves/").Length;
        for (int i = 0; i < names.Length; i++)
        {
            names[i] = names[i].Substring(offset);
        }

        return names;
    }

    public void Save(string data)
    {
        SaveFile(data, DataPath);
    }

    public string Load()
    {
        return LoadFile(DataPath);
    }

    public string LoadSettings()
    {
        return LoadFile(SettingsPath);
    }

    public void SaveSettings(string data)
    {
        SaveFile(data, SettingsPath);
    }

    private static void SaveFile(string data, string path)
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

    private static string LoadFile(string _path)
    {
        string data;

        Debug.Log("Loading: " + _path);

        if (!File.Exists(_path))
        {
            Debug.Log("File not found: " + _path);
            return null;
        }

        data = File.ReadAllText(_path);

        return data;
    }

    internal void DeleteCurrentProfile()
    {
        if (Directory.Exists(currentProfileFolder)) { Directory.Delete(currentProfileFolder, true); }
    }
}
