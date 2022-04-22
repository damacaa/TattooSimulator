using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager
{

    public static string root;
    public static string folderPath;
    public static string fileName;

    public static string Path
    {
        get { return folderPath + fileName; }
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
        fileName = profile + ".txt";
        folderPath = root + "/Saves/" + profile + "/";
    }

    public static void Save(string data)
    {

        try
        {


#if UNITY_STANDALONE || UNITY_EDITOR
            Helpers.SafeCreateDirectory(folderPath);
            File.WriteAllText(Path, data);


#elif UNITY_ANDROID
            Helpers.SafeCreateDirectory(folderPath);
            StreamWriter Writer = new StreamWriter(Path);
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

    public static string Load()
    {
        string data;

        Debug.Log("Loading: " + Path);

        if (!File.Exists(Path))
        {
            Debug.Log("File not found: " + Path);
            return null;
        }

        data = File.ReadAllText(Path);

        return data;
    }

    public static string LoadSettings()
    {
        string data;

        if (!File.Exists(SettingsPath))
        {
            Debug.Log("Settings not found: " + SettingsPath);
            return null;
        }

        data = File.ReadAllText(SettingsPath);

        return data;
    }

    public static void SaveSettings(string data)
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        File.WriteAllText(SettingsPath, data);
#elif UNITY_ANDROID
        //Helpers.SafeCreateDirectory(folderPath);
        StreamWriter Writer = new StreamWriter(SettingsPath);
        Writer.Write(data);
        Writer.Flush();
        Writer.Close();
#endif
    }

    private static void SaveFile()
    {

    }

    private static void LoadFile()
    {

    }


}
