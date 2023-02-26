using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Creates and destroys tattoos.
/// </summary>
public class TattooSpawner : MonoBehaviour
{
    static public TattooSpawner Instance;

    private void Awake()
    {
        Instance = this;
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
        currentTattooTexture = DesignManager.Instance.GetDesign(s);
        UI.UIManager.instance.ShowSettings();
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
        GameObject g = SpawnTattoo(pos, rot, currentTattooTexture, TattooEditor.Instance.Size).gameObject;

        lastTattoo = g;
        currentTattooTexture = null;

        HideCursor();
        UI.UIManager.instance.HideSettings();
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
        rot.z = TattooEditor.Instance.Angle;
        cursor.transform.localScale = new Vector3(TattooEditor.Instance.Size, TattooEditor.Instance.Size, 0.05f);
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
        TattooEditor.Instance.Reset();

        currentTattooTexture = null;
        lastTattoo = null;
        foreach (SmartTattoo t in spawnedTattoos)
        {
            Destroy(t.gameObject);
        }
        spawnedTattoos.Clear();
        UI.UIManager.instance.HideSettings();
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
        Texture t = DesignManager.Instance.GetDesign(design);

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
}
