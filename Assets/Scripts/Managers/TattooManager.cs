using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TattooManager : MonoBehaviour
{
    static public TattooManager instance;

    private void Awake()
    {
        instance = this;
        HideCursor();
    }
    [SerializeField]
    string fileName = "data";
    [SerializeField]
    GameObject tattooPrefab;
    [SerializeField]
    GameObject cursor;

    Texture currentTattooTexture;
    GameObject lastTattoo;

    SmartTattoo selectedTattoo;
    List<SmartTattoo> spawnedTattoos = new List<SmartTattoo>();

    float angle = 0;
    float size = 0.1f;

    public void SetAngle(float a)
    {
        angle = -a;
    }

    public void SetSize(float s)
    {
        size = s;
    }

    public void SetTexture(Texture t)
    {
        currentTattooTexture = t;
        UIManager.instance.ShowSettings();
    }

    public void PrintTattoo(Vector3 pos, Vector3 normal)
    {
        if (!currentTattooTexture)
        {
            //print("No tattoo selected");
            return;
        }

        //pos = new Vector3(0, 1.2f, -1);
        //normal = new Vector3(0, 0, -1);

        GameObject g = GameObject.Instantiate(tattooPrefab, pos, Quaternion.LookRotation(-normal, Camera.main.transform.up));

        Vector3 rot = g.transform.rotation.eulerAngles;
        rot.z += angle;
        g.transform.rotation = Quaternion.Euler(rot);
        g.transform.localScale = new Vector3(size, size, 1);

        SmartTattoo smartTattoo = g.GetComponent<SmartTattoo>();
        smartTattoo.texture = currentTattooTexture;
        spawnedTattoos.Add(smartTattoo);

        lastTattoo = g;
        currentTattooTexture = null;

        HideCursor();
        UIManager.instance.HideSettings();
        Save();

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
        rot.z = angle;
        cursor.transform.localScale = new Vector3(size, size, 0.05f);
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
            Save();
        }
    }

    public void SelectTattoo(SmartTattoo tattoo)
    {
        selectedTattoo = tattoo;
        UIManager.instance.ShowSettings();
        UIManager.instance.SetSettings(0, tattoo.transform.localScale.x);
        StartCoroutine(AdjustTattoo());
    }

    IEnumerator AdjustTattoo()
    {
        float lastSize = size;
        float lastAngle = angle;

        Vector3 rot = selectedTattoo.transform.rotation.eulerAngles;
        float defaultZ = rot.z;

        bool hasChanges = false;
        SmartTattoo t = selectedTattoo;
        while (selectedTattoo != null && selectedTattoo == t)
        {
            //Vector3 rot = selectedTattoo.transform.rotation.eulerAngles;
            //rot.z += angle;
            //g.transform.rotation = Quaternion.Euler(rot);
            if (angle != lastAngle)
            {
                if (!hasChanges)
                {
                    selectedTattoo.Reset();
                    hasChanges = true;
                }

                lastAngle = angle;
                rot.z = defaultZ + angle;
                selectedTattoo.transform.rotation = Quaternion.Euler(rot);
            }

            if (size != lastSize)
            {
                if (!hasChanges)
                {
                    selectedTattoo.Reset();
                    hasChanges = true;
                }

                lastSize = size;
                selectedTattoo.transform.localScale = new Vector3(size, size, 1);
                selectedTattoo.AdjustScale();
            }
            yield return null;
        }

        if (hasChanges && t)
        {
            t.Begin();
            Save();
        }

        yield return null;
    }

    public void Deselect()
    {
        selectedTattoo = null;
        UIManager.instance.HideSettings();
    }

    public void DeleteSelected()
    {
        if (selectedTattoo)
        {
            spawnedTattoos.Remove(selectedTattoo);
            Destroy(selectedTattoo.gameObject);
            UIManager.instance.HideSettings();
        }
    }

    private void Start()
    {
        LoadData();
    }

    List<TattooInfo> unableToLoadTattoos = new List<TattooInfo>();
    public void LoadData()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        if (!File.Exists(Application.dataPath + "/" + fileName + ".txt"))
            return;

        string data = File.ReadAllText(Application.dataPath + "/" + fileName + ".txt");


        TattoList tattoList = JsonUtility.FromJson<TattoList>(data);
        foreach (TattooInfo t in tattoList.tattoos)
        {
            Texture tex;
            if (!Content.instance.textures.TryGetValue(t.texture, out tex))
            {
                unableToLoadTattoos.Add(t);
                print("Unable to load " + t.texture);
                continue;
            }

            GameObject g = GameObject.Instantiate(tattooPrefab, t.position, Quaternion.Euler(t.euler));
            g.transform.localScale = new Vector3(t.size, t.size, 1);
            SmartTattoo smartTattoo = g.GetComponent<SmartTattoo>();
            smartTattoo.texture = tex;
            spawnedTattoos.Add(smartTattoo);
        }
#endif
    }

    private void Save()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        List<TattooInfo> tattooInfos = new List<TattooInfo>();
        foreach (SmartTattoo smartTattoo in spawnedTattoos)
        {
            tattooInfos.Add(smartTattoo.GetInfo());
        }

        tattooInfos.AddRange(unableToLoadTattoos);

        TattoList tattooList = new TattoList();
        tattooList.tattoos = tattooInfos.ToArray();

        string json = JsonUtility.ToJson(tattooList);
        File.WriteAllText(Application.dataPath + "/" + fileName + ".txt", json);
#endif
    }

    [System.Serializable]
    class TattoList
    {
        public TattooInfo[] tattoos;
    }

}
