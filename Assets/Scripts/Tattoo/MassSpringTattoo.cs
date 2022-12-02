using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MassSpringTattoo : SmartTattoo
{
    protected override void UpdateMesh(float h)
    {
        for (int i = 0; i < springs.Count; i++)
        {
            springs[i].Update(h);
            //springs[i].ResetLength();
        }


        Vector3[] v = new Vector3[nodes.Length];

        done = true;
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].Update(h, thickness);
            done = done && nodes[i].isFixed;
            v[i] = transform.InverseTransformPoint(nodes[i].position);
        }
        mesh.vertices = v;
    }
}
