using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sticker : MonoBehaviour
{
    [SerializeField]
    Vector3 dir = Vector3.forward;
    [SerializeField]
    float thickness = 0.01f;

    bool[] isFixed;
    Mesh mesh;
    bool firstContact = false;
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        isFixed = new bool[mesh.vertexCount];
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            isFixed[i] = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*if (!firstContact)
        {
            transform.position += Time.deltaTime * dir;
            return;
        }*/

        Vector3[] v = mesh.vertices;
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            if (isFixed[i])
            {
                //v[i] = mesh.vertices[i];
                continue;
            }

            Vector3 currentPos = transform.TransformPoint(mesh.vertices[i]);
            Vector3 nextPos = currentPos + Time.deltaTime * dir;

            RaycastHit hit;
            if (Physics.Raycast(currentPos, dir, out hit, Time.deltaTime + thickness))
            {
                firstContact = true;
                Debug.Log("Fix");
                isFixed[i] = true;
                continue;
            }

            v[i] = transform.InverseTransformPoint(nextPos);
        }

        if (firstContact)
            mesh.vertices = v;
        else
            transform.position += dir * Time.deltaTime;
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Contact");
        firstContact = true;
    }*/
}
