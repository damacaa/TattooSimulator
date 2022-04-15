using System.Collections;
using System.Collections.Generic;
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
    GameObject tattooPrefab;
    [SerializeField]
    GameObject cursor;

    Texture currentTattooTexture;
    GameObject lastTattoo;

    float angle = 0;
    float size = 0.1f;

    public void SetAngle(float a)
    {
        angle = -a;
    }

    public void SetSize(float s)
    {
        size = s / 100f;
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
            print("No tattoo selected");
            return;
        }

        //pos = new Vector3(0, 1.2f, -1);
        //normal = new Vector3(0, 0, -1);

        GameObject g = GameObject.Instantiate(tattooPrefab, pos, Quaternion.LookRotation(-normal, Vector3.up));

        Vector3 rot = g.transform.rotation.eulerAngles;
        rot.z = angle;
        g.transform.rotation = Quaternion.Euler(rot);

        g.transform.localScale = new Vector3(size, size, 1);

        g.GetComponent<SmartTattoo>().texture = currentTattooTexture;
        lastTattoo = g;
        currentTattooTexture = null;

        HideCursor();
        UIManager.instance.HideSettings();

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
            Destroy(lastTattoo);
    }
}
