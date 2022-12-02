using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshColliderUpdate : MonoBehaviour
{
    // Start is called before the first frame update
    MeshCollider meshCollider;
    Mesh mesh;
    SkinnedMeshRenderer renderer;
    void Start()
    {
        print(name);

        meshCollider = GetComponent<MeshCollider>();
        renderer = GetComponentInChildren<SkinnedMeshRenderer>();

        mesh = renderer.sharedMesh;
        meshCollider.sharedMesh = mesh;
    }

    IEnumerator a()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(1f);

            UpdateCollider();
            

            GameObject g = new GameObject();
            g.transform.position = Vector3.zero;
            g.AddComponent<MeshFilter>().mesh = mesh;
            g.AddComponent<MeshRenderer>().material = renderer.material;
            g.transform.rotation = transform.rotation;

           // meshCollider.sharedMesh = mesh;

        }
        yield return null;
    }

    public void UpdateCollider()
    {
        mesh = new Mesh();
        renderer.BakeMesh(mesh);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
    }

}
