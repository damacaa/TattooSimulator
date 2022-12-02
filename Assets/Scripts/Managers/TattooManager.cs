using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class TattooManager : MonoBehaviour
{
    static public TattooManager instance;

    private void Awake()
    {
        instance = this;
        HideCursor();
    }

    [SerializeField]
    GameObject tattooPrefab;
    [SerializeField]
    GameObject cursor;

    Texture currentTattooTexture;
    GameObject lastTattoo;

    public List<SmartTattoo> spawnedTattoos = new List<SmartTattoo>();


    public void SetTexture(string s)
    {
        currentTattooTexture = DesignManager.instance.GetDesign(s);
        UIManager.instance.ShowSettings();
    }

    public void PrintTattoo(Vector3 pos, Vector3 forward)
    {
        if (!currentTattooTexture)
        {
            //print("No tattoo selected");
            return;
        }

        //pos = new Vector3(0, 1.2f, -1);
        //normal = new Vector3(0, 0, -1);

        Vector3 rot = Quaternion.LookRotation(forward, Camera.main.transform.up).eulerAngles;
        rot.z = 0;
        GameObject g = SpawnTattoo(pos, rot, currentTattooTexture, TattooEditor.instance.size).gameObject;

        lastTattoo = g;
        currentTattooTexture = null;

        HideCursor();
        UIManager.instance.HideSettings();
        ProfileManager.instance.Save();

#if UNITY_EDITOR
        Selection.activeObject = g;
#endif
    }

    public void Preview(Vector3 pos, Vector3 normal)
    {
        if (!currentTattooTexture)
            return;

        cursor.SetActive(true);
        cursor.transform.position = pos;
        cursor.transform.forward = -normal;
        Vector3 rot = cursor.transform.rotation.eulerAngles;
        rot.z = TattooEditor.instance.angle;
        cursor.transform.localScale = new Vector3(TattooEditor.instance.size, TattooEditor.instance.size, 0.05f);
        cursor.transform.rotation = Quaternion.Euler(rot);
    }

    public void HideCursor()
    {
        cursor.SetActive(false);
    }

    public void Undo()
    {
        if (lastTattoo)
        {
            spawnedTattoos.Remove(lastTattoo.GetComponent<SmartTattoo>());
            Destroy(lastTattoo);
            ProfileManager.instance.Save();
        }
    }


    public void DeleteTattoo(SmartTattoo tattoo)
    {
        spawnedTattoos.Remove(tattoo);
        Destroy(tattoo.gameObject);
    }




    public void Reset()
    {
        TattooEditor.instance.Reset();

        currentTattooTexture = null;
        lastTattoo = null;
        foreach (SmartTattoo t in spawnedTattoos)
        {
            Destroy(t.gameObject);
        }
        spawnedTattoos.Clear();
        UIManager.instance.HideSettings();
    }

    public SmartTattoo SpawnTattoo(Vector3 pos, Vector3 rotation, Texture texture, float size)
    {
        GameObject g = GameObject.Instantiate(tattooPrefab, pos, Quaternion.Euler(rotation));

        SmartTattoo smartTattoo = g.GetComponent<SmartTattoo>();
        smartTattoo.texture = texture;
        smartTattoo.Size = size;

        spawnedTattoos.Add(smartTattoo);

        return smartTattoo;
    }

    internal void DeleteTattoosWithDesign(string design)
    {
        Texture t = DesignManager.instance.GetDesign(design);

        List<SmartTattoo> tattosToRemove = spawnedTattoos.FindAll(HasSameDesing);

        foreach (var s in tattosToRemove)
        {
            Destroy(s.gameObject);
            spawnedTattoos.Remove(s);
        }

        bool HasSameDesing(SmartTattoo st)
        {
            return st.texture = t;
        }
    }

    /////////////////////// THIS SHOULD BE ANOTHER SCRIPT

}
