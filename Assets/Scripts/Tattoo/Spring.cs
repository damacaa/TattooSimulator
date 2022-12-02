using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring
{
    public Node a, b;
    float length0;
    float length;
    Vector3 dir0;
    SmartTattoo tattoo;

    public float LengthDifference
    {
        get
        {
            return length / length0;
        }
    }

    public Spring(Node a, Node b, SmartTattoo tattoo)
    {
        this.a = a;
        this.b = b;
        this.tattoo = tattoo;

        dir0 = b.position - a.position;
        length0 = Vector3.Distance(a.position, b.position);
    }

    public void Update(float h)
    {
        if (a.isFixed && b.isFixed)
            return;

        Vector3 u = a.position - b.position;

        length = u.magnitude;
        u.Normalize();

        Vector3 force = -tattoo.stiffness * (length - length0) * u;

        a.force += force;
        b.force -= force;


        /*if (a.isFixed || b.isFixed)
        {
            Node fixedNode = a.isFixed ? a : b;
            Node notFixedNode = a.isFixed ? b : a;

            notFixedNode.position = fixedNode.position + ((0.1f * length0 + 0.9f * length) * (notFixedNode.position - fixedNode.position).normalized);
        }*/




        //Vector3 axis = Vector3.Cross(fixedNode.direction, b.position - a.position);

        //notFixedNode.direction = Quaternion.AngleAxis(10f, Vector3.up) * fixedNode.direction;
        //notFixedNode.direction = ((fixedNode.position + fixedNode.direction) - difference*notFixedNode.position).normalized;
        //fixedNode.direction *= -1;

    }

    public void ResetLength()
    {
       if (a.isFixed && b.isFixed)
            return;

        Vector3 u = a.position - b.position;
        length = u.magnitude;
        float variation = length - length0;

        if (!a.isFixed && !b.isFixed)
        {
            a.position += u * (variation / 2f);
            b.position -= u * (variation / 2f);
        }
        if ((a.isFixed && !b.isFixed) || (!a.isFixed && b.isFixed))
        {
            Node fixedNode = a.isFixed ? a : b;
            Node notFixedNode = a.isFixed ? b : a;

            notFixedNode.position = fixedNode.position + ((0.1f * length0 + 0.9f * length) * (notFixedNode.position - fixedNode.position).normalized);
        }
    }
}
