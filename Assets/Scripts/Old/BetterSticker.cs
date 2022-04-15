using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BetterSticker : MonoBehaviour
{
    [SerializeField]
    Texture texture;
    [SerializeField]
    int accuracy = 1;
    [SerializeField]
    int divisions = 3;

    Vector3 dir = Vector3.forward;
    bool[] isFixed;
    Mesh mesh;
    bool firstContact = false;
    // Start is called before the first frame update
    void Start()
    {
        float localwidth = 1;
        float localHeight = 1;

        int verticesPerFace = divisions + 2;
        Vector3[] vertices = new Vector3[verticesPerFace * verticesPerFace];
        Vector2[] uv = new Vector2[verticesPerFace * verticesPerFace];

        for (int i = 0; i < verticesPerFace; i++)
        {
            for (int j = 0; j < verticesPerFace; j++)
            {
                float x = -(localwidth / 2f) + i * (localwidth / (verticesPerFace - 1));
                float y = -(localHeight / 2f) + j * (localHeight / (verticesPerFace - 1));
                vertices[(verticesPerFace * j) + i] = new Vector3(x, y, 0);
                //vertices[(verticesPerFace * j) + i] = new Vector3(x, 0, y);//Horizontal plane

                float u = (x + (localwidth / 2f)) / localwidth;
                float v = (y + (localHeight / 2f)) / localHeight;
                uv[(verticesPerFace * j) + i] = new Vector2(u, v);
            }
        }

        //Triangles
        List<int> t = new List<int>();

        for (int i = 0; i < verticesPerFace - 1; i++)
        {
            for (int j = 0; j < verticesPerFace - 1; j++)
            {
                //Clockwise
                t.Add((verticesPerFace * j) + i);
                t.Add((verticesPerFace * (j + 1)) + i);
                t.Add((verticesPerFace * (j + 1)) + i + 1);

                t.Add((verticesPerFace * j) + i);
                t.Add((verticesPerFace * (j + 1)) + i + 1);
                t.Add((verticesPerFace * j) + i + 1);
            }
        }

        mesh = new Mesh();
        mesh.name = "Plane " + verticesPerFace + "x" + verticesPerFace;
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = t.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();


        //////////////////////
        isFixed = new bool[mesh.vertexCount];
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            isFixed[i] = false;
        }

        dir = transform.forward;

        GetComponent<MeshRenderer>().material.mainTexture = texture;
    }

    private void FixedUpdate()
    {
        float h = Time.deltaTime / accuracy;

        Vector3[] v = mesh.vertices;
        int totalFixed = 0;
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            if (isFixed[i])
            {
                totalFixed++;

                //v[i] = mesh.vertices[i];
                continue;
            }

            Vector3 currentPos = transform.TransformPoint(mesh.vertices[i]);
            Vector3 nextPos = currentPos + h * dir;

            RaycastHit hit;
            if (Physics.Raycast(currentPos, dir, out hit, h))
            {
                firstContact = true;
                isFixed[i] = true;
                continue;
            }

            v[i] = transform.InverseTransformPoint(nextPos);
        }

        if (firstContact)
            mesh.vertices = v;
        else
            transform.position += dir * h;

        if (totalFixed == mesh.vertexCount)
        {
            gameObject.AddComponent<BoxCollider>();
            enabled = false;
            return;
        }

    }

    private void OnDrawGizmos()
    {
        float width = transform.localScale.x;
        float heigth = width * (float)texture.height / (float)texture.width;//Not working perfectly, texture width and height don't match picture's

        transform.localScale = new Vector3(width, heigth, 1);

        /*if (!mesh)
            return;
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            Handles.Label(transform.TransformPoint(mesh.vertices[i]), i.ToString());
        }*/
    }

    void OnMouseDown()
    {
        CameraBehaviour.instance.MoveTargetTo(transform.position);
    }
}
