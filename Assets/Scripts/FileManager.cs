using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager
{
    public static string folderPath;
    public static string fileName;

    public static string Path
    {
        get { return folderPath + fileName; }
    }

    public static void SetProfile(string profile)
    {
        fileName = profile + ".txt";

#if UNITY_STANDALONE || UNITY_EDITOR

        folderPath = Application.dataPath + "/Saves/";

#else

        folderPath = Application.persistentDataPath + "/Saves/";

#endif
    }

    public static void Save(string data)
    {


#if UNITY_STANDALONE || UNITY_EDITOR
        File.WriteAllText(Path, data);


#elif UNITY_ANDROID
        Helpers.SafeCreateDirectory(folderPath);
        StreamWriter Writer = new StreamWriter(Path);
        Writer.Write(data);
        Writer.Flush();
        Writer.Close();
#endif


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
#if UNITY_STANDALONE || UNITY_EDITOR

#elif UNITY_ANDROID

       /* var reader = new StreamReader(Path);
        data = reader.ReadToEnd();
        reader.Close();*/

#else

        return null;

#endif
        return data;
    }
}
