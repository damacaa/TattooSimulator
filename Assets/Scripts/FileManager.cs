using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager
{

    public static string root;
    public static string folderPath;

    public static string DataPath
    {
        get { return folderPath + "data.txt"; }
    }

    public static string ImageFolderPath
    {
        get {
            string path = folderPath + "/images";
            Helpers.SafeCreateDirectory(path);
            return path; }
    }

    public static string SettingsPath
    {
        get { return root + "/settings.txt"; }
    }


    public static void Initialize()
    {
#if UNITY_STANDALONE || UNITY_EDITOR

        root = Application.dataPath;
#else

        root = Application.persistentDataPath;
#endif
        Helpers.SafeCreateDirectory(root + "/Saves");
    }

    public static void SetProfile(string profile)
    {
        folderPath = root + "/Saves/" + profile + "/";
    }

    public static void Save(string data)
    {
        SaveFile(data, DataPath);
    }

    public static string Load()
    {
        return LoadFile(DataPath);
    }

    public static string LoadSettings()
    {
        return LoadFile(SettingsPath);
    }

    public static void SaveSettings(string data)
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

    internal static void DeleteCurrentProfile()
    {
        if (Directory.Exists(folderPath)) { Directory.Delete(folderPath, true); }
    }
}
