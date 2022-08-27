using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TattooEditor : MonoBehaviour
{
    public static TattooEditor instance;

    private void Awake()
    {
        instance = this;
    }

    SmartTattoo selectedTattoo;

    public float angle = 0;
    public float size = 0.1f;

    public void SetAngle(float a)
    {
        angle = -a;
    }

    public void SetSize(float s)
    {
        size = s;
    }

    public void SelectTattoo(SmartTattoo tattoo)
    {
        selectedTattoo = tattoo;

        UIManager.instance.ShowSettings();
        UIManager.instance.SetSettings(0, tattoo.transform.localScale.x);
        StartCoroutine(AdjustTattoo());

#if UNITY_EDITOR
        Selection.activeObject = tattoo;
#endif
    }

    IEnumerator AdjustTattoo()
    {
        float lastSize = size;
        float lastAngle = angle;

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
        UIManager.instance.HideSettings();
    }

    public void DeleteSelected()
    {
        if (selectedTattoo)
        {
            TattooManager.instance.DeleteTattoo(selectedTattoo);
            UIManager.instance.HideSettings();
            ProfileManager.instance.Save();
        }
    }

    internal void MoveTattoo(Vector3 pos, Vector3 forward)
    {
        selectedTattoo.GetComponent<Collider>().enabled = false;
        selectedTattoo.transform.position = pos - 0.005f * forward;
        //selectedTattoo.transform.forward = forward;
        selectedTattoo.transform.rotation = Quaternion.LookRotation(forward, selectedTattoo.transform.up);
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
    }
}
