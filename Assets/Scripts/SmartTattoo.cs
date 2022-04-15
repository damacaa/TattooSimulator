using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public abstract class SmartTattoo : MonoBehaviour
{
    class Quad
    {
        public int a, b, c, d;
        public bool empty = true;
    }

    class Edge
    {
        public int a, b, c;

        public Edge(int a, int b, int c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }
    }
    class EdgeComparer : IComparer<Edge>
    {
        public int Compare(Edge e1, Edge e2)
        {
            if (e1.a < e2.a)
            {
                return 1;
            }
            else
            {
                if (e1.a <= e2.a)
                {
                    if (e1.b < e2.b) { return 1; }
                    else if (e1.b > e2.b) { return -1; }
                    else { return 0; }
                }
                else
                {
                    return -1;
                }
            }
        }
    }

    //Parameters
    [SerializeField]
    public Texture texture;
    [SerializeField]
    protected int accuracy = 10;
    [SerializeField]
    protected int divisions = 10;
    [SerializeField]
    protected float thickness = 0.003f;
    [SerializeField]
    internal float nodeMass = 0.001f;
    [SerializeField]
    internal float stiffness = 1f;

    //Mesh info
    protected int verticesPerFace;
    protected int verticesPerWidth, verticesPerHeight;
    [SerializeField]
    private bool removeEmptySpaces = false;

    //Simulation
    protected Node[] nodes;
    protected List<Spring> springs;

    protected Mesh mesh;

    void Start()
    {
        gameObject.tag = "Tattoo";
        name = "Tatto: " + texture.name;
        AdjustScale();
        GetComponent<MeshRenderer>().material.mainTexture = texture;

        int a = 0;
        float step = Time.fixedDeltaTime / accuracy;
        Vector3 pos = transform.position;

        while (a < 1000 && Physics.CheckBox(pos, new Vector3(transform.localScale.x, transform.localScale.y, 0.05f) / 4f, transform.rotation))
        {
            pos -= step * transform.forward;
            a++;
        }

        transform.position = pos;

        BuildMesh();
    }

    protected bool done = false;


    private void Update()
    {
        float h = Time.fixedDeltaTime / accuracy;

        for (int t = 0; t < Mathf.Sqrt(accuracy); t++)
        {
            UpdateMesh(h);
            stiffness = Mathf.Max(0.5f, stiffness - (10f * h));

            if (done)
            {
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                mesh.RecalculateTangents();
                gameObject.AddComponent<MeshCollider>();
                Print();
                enabled = false;

                Debug.Log(name + " done");
            }
        }
    }

    protected abstract void UpdateMesh(float h);

    protected void AdjustScale()
    {
        float width = transform.localScale.x;
        float heigth = width * (float)texture.height / (float)texture.width;//Not working perfectly, texture width and height don't match picture's

        transform.localScale = new Vector3(width, heigth, 1f);
    }

    protected void BuildMesh()
    {
        verticesPerFace = divisions + 2;
        if (transform.localScale.x > transform.localScale.y)
        {
            verticesPerWidth = verticesPerFace;
            verticesPerHeight = Mathf.Max(3, Mathf.RoundToInt(verticesPerFace * (transform.localScale.y / transform.localScale.x)));
        }
        else
        {
            verticesPerWidth = Mathf.Max(3, Mathf.RoundToInt(verticesPerFace * (transform.localScale.x / transform.localScale.y)));
            verticesPerHeight = verticesPerFace;
        }

        float localwidth = 1;
        float localHeight = 1;

        Vector3[] vertices = new Vector3[verticesPerWidth * verticesPerHeight];
        Vector2[] uv = new Vector2[verticesPerWidth * verticesPerHeight];

        nodes = new Node[verticesPerWidth * verticesPerHeight];
        springs = new List<Spring>();

        //Vertices
        for (int i = 0; i < verticesPerWidth; i++)
        {
            for (int j = 0; j < verticesPerHeight; j++)
            {
                float x = -(localwidth / 2f) + i * (localwidth / (verticesPerWidth - 1));
                float y = -(localHeight / 2f) + j * (localHeight / (verticesPerHeight - 1));

                //Nodes
                Vector3 pos = new Vector3(x, y, 0);//new Vector3(x, 0, y);//Horizontal plane
                vertices[(verticesPerWidth * j) + i] = pos;
                nodes[(verticesPerWidth * j) + i] = new Node(transform.TransformPoint(pos), transform.forward, nodeMass);

                //UVs
                float u = (x + (localwidth / 2f)) / localwidth;
                float v = (y + (localHeight / 2f)) / localHeight;
                uv[(verticesPerWidth * j) + i] = new Vector2(u, v);
            }
        }

        //Quads
        Texture2D tex2D = (Texture2D)texture;
        Quad[,] quads = new Quad[verticesPerWidth - 1, verticesPerHeight - 1];

        for (int i = 0; i < verticesPerWidth - 1; i++)
        {
            for (int j = 0; j < verticesPerHeight - 1; j++)
            {
                Quad q = new Quad();
                //Clockwise
                q.a = (verticesPerWidth * j) + i;
                q.b = (verticesPerWidth * (j + 1)) + i;
                q.c = (verticesPerWidth * (j + 1)) + i + 1;
                q.d = (verticesPerWidth * j) + i + 1;
                q.empty = true;

                Vector2 point = uv[q.a];

                const int step = 3;
                for (int u = 0; u < tex2D.width / (verticesPerWidth - 1); u += step)
                {
                    for (int v = 0; v < tex2D.height / (verticesPerHeight - 1); v += step)
                    {
                        if (tex2D.GetPixel((int)(point.x * tex2D.width) + u, (int)(point.y * tex2D.height) + v) != Color.clear)
                        {
                            q.empty = false;
                            break;
                        }
                    }
                    if (!q.empty)
                        break;
                }

                quads[i, j] = q;

                /*t.Add(a);
                t.Add(b);
                t.Add(c);

                t.Add(a);
                t.Add(c);
                t.Add(d);*/

                /*springs.Add(new Spring(nodes[a], nodes[b], this));
                springs.Add(new Spring(nodes[b], nodes[c], this));
                springs.Add(new Spring(nodes[c], nodes[a], this));
                springs.Add(new Spring(nodes[b], nodes[d], this));//Optional*/
            }
        }

        //Triangles
        List<int> t = new List<int>();
        for (int i = 0; i < verticesPerWidth - 1; i++)
        {
            for (int j = 0; j < verticesPerHeight - 1; j++)
            {
                Quad q = quads[i, j];
                if (removeEmptySpaces)
                {
                    bool allNeighboursEmptyOrOut = true;
                    //Find neighbours
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            if (Mathf.Abs(x) + Mathf.Abs(y) != 1)
                                continue;

                            int ix = i + x;
                            int jy = j + y;

                            if (ix < 0 || jy < 0 || ix >= verticesPerWidth - 1 || jy >= verticesPerHeight - 1)
                                continue;

                            if (!quads[ix, jy].empty)
                            {
                                allNeighboursEmptyOrOut = false;
                                break;
                            }
                        }
                        if (!allNeighboursEmptyOrOut)
                            break;
                    }

                    if (q.empty && allNeighboursEmptyOrOut)
                        continue;
                }

                t.Add(q.a);
                t.Add(q.b);
                t.Add(q.c);

                t.Add(q.a);
                t.Add(q.c);
                t.Add(q.d);
            }
        }

        /*for (int i = 0; i < verticesPerWidth - 1; i++)
        {
            //springs.Add(new Spring(nodes[i], nodes[i + 1], this));
        }

        for (int i = 0; i < verticesPerHeight - 1; i++)
        {
            //springs.Add(new Spring(nodes[(i * verticesPerHeight) + verticesPerWidth - 1], nodes[((i + 1) * verticesPerHeight) + verticesPerWidth - 1],this));
        }*/

        List<Edge> edges = new List<Edge>();
        EdgeComparer edgeComparer = new EdgeComparer();
        for (int i = 0; i < t.Count - 1; i += 3)
        {
            edges.Add(new Edge(t[i], t[i + 1], t[i + 2]));
            edges.Add(new Edge(t[i], t[i + 2], t[i + 1]));
            edges.Add(new Edge(t[i + 1], t[i + 2], t[i]));
        }

        edges.Sort(edgeComparer);

        //Crea muelles
        Edge lastEdge = new Edge(0, 0, 0);
        foreach (Edge e in edges)
        {
            if (edgeComparer.Compare(lastEdge, e) == 0)
            {
                //Ya existe el muelle
                //Se crea un muelle de flexion
                springs.Add(new Spring(nodes[lastEdge.c], nodes[e.c], this));
            }
            else
            {
                springs.Add(new Spring(nodes[e.a], nodes[e.b], this));
            }
            lastEdge = e;
        }

        /*foreach (Edge e in edges)
        {
            springs.Add(new Spring(e.a, e.b, this));
        }*/

        foreach (Spring s in springs)
        {
            s.a.isFixed = false;
            s.b.isFixed = false;
        }

        mesh = new Mesh();
        mesh.name = "Plane " + verticesPerWidth + "x" + verticesPerHeight;
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = t.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
    }
    //https://www.youtube.com/watch?v=f7iO9ernEmM
    protected void Print()
    {

    }

    protected void OnMouseDown()
    {
        CameraBehaviour.instance.MoveTargetTo(transform.position);
#if UNITY_EDITOR
        Selection.activeObject = this;
#endif
    }

#if UNITY_EDITOR
    protected void OnDrawGizmosSelected()
    {
        if (transform.hasChanged && texture)
        {
            AdjustScale();
        }

        if (!mesh)
            return;

        for (int i = 0; i < nodes.Length; i++)
        {
            if (!nodes[i].isFixed)
                Handles.Label((nodes[i].position), i.ToString());
        }


        for (int i = 0; i < springs.Count; i++)
        {
            float a = springs[i].LengthDifference;
            if (a > 1.01f)
            {
                a = springs[i].LengthDifference - 0.5f;

                Gizmos.color = new Color(a, 1f - a, 1f - a, a);
            }
            else if (a < 0.99f)
            {
                a = springs[i].LengthDifference - 0.5f;

                Gizmos.color = new Color(1f - a, 1f - a, a, a);
            }

            Gizmos.DrawLine(springs[i].a.position, springs[i].b.position);
        }
    }

    protected void OnValidate()
    {
        if (texture)
        {
            name = "Tatto: " + texture.name;
            AdjustScale();
            //GetComponent<MeshRenderer>().material.mainTexture = texture;
        }
    }
#endif
}


