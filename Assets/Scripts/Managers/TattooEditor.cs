using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TattooEditor : MonoBehaviour
{
    public static TattooEditor Instance;
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField]
    UI.TattooEditorUI _ui;

    public float Angle { get; private set; }
    public float Size { get; private set; }
    SmartTattoo selectedTattoo;

    private void Start()
    {
        Size = 0.1f;
        _ui.SetSettings(Angle, Size);
    }

    public void SetAngle(float a)
    {
        Angle = -a;
    }

    public void SetSize(float s)
    {
        Size = s;
    }

    public void SelectTattoo(SmartTattoo tattoo)
    {
        selectedTattoo = tattoo;

        UI.UIManager.instance.ShowSettings();
        _ui.SetSettings(tattoo.Angle, tattoo.Size);
        StartCoroutine(AdjustTattoo());

#if UNITY_EDITOR
        Selection.activeObject = tattoo;
#endif
    }

    IEnumerator AdjustTattoo()
    {
        float lastSize = Size;
        float lastAngle = Angle;

        Vector3 rot = selectedTattoo.transform.rotation.eulerAngles;
        float defaultZ = rot.z;

        bool hasChanges = false;
        int framesWithoutChanges = 0;
        SmartTattoo t = selectedTattoo;
        while (selectedTattoo != null && selectedTattoo == t)
        {
            //Vector3 rot = selectedTattoo.transform.rotation.eulerAngles;
            //rot.z += angle;
            //g.transform.rotation = Quaternion.Euler(rot);
            if (Angle != lastAngle)
            {
                if (!hasChanges)
                {
                    selectedTattoo.Reset();
                    hasChanges = true;
                }

                lastAngle = Angle;
                selectedTattoo.Angle = Angle;
            }

            if (Size != lastSize)
            {
                if (!hasChanges)
                {
                    selectedTattoo.Reset();
                    hasChanges = true;
                }

                lastSize = Size;
                selectedTattoo.Size = Size;
            }

            if (!hasChanges)
            {
                framesWithoutChanges++;
                if (framesWithoutChanges == 60)
                {
                    //print("Go");
                    //selectedTattoo.Begin();
                }
            }
            else
                framesWithoutChanges = 0;

            yield return null;
        }

        if (hasChanges && t)
        {
            t.Begin();
            ProfileManager.instance.Save();
        }

        yield return null;
    }

    public void Deselect()
    {
        selectedTattoo = null;
        UI.UIManager.instance.HideSettings();
    }

    public void DeleteSelected()
    {
        if (selectedTattoo)
        {
            TattooSpawner.Instance.DeleteTattoo(selectedTattoo);
            UI.UIManager.instance.HideSettings();
            ProfileManager.instance.Save();
        }
    }

    internal void MoveTattoo(Vector3 pos, Vector3 forward)
    {
        selectedTattoo.GetComponent<Collider>().enabled = false;
        selectedTattoo.transform.position = pos - 0.005f * forward;
        //selectedTattoo.transform.forward = forward;
        Vector3 rot = Quaternion.LookRotation(forward, selectedTattoo.transform.up).eulerAngles;
        rot.z = 0;
        selectedTattoo.transform.rotation = Quaternion.Euler(rot);
        selectedTattoo.Reset();
    }

    internal void StopMovingTattoo()
    {
        if (selectedTattoo)
        {
            selectedTattoo.GetComponent<Collider>().enabled = true;
            selectedTattoo.Begin();
            ProfileManager.instance.Save();
        }
    }

    public void Reset()
    {
        selectedTattoo = null;
        _ui.SetSettings(0, .1f);
    }
}
