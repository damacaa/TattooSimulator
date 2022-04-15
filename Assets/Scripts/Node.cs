using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node
{
    public bool isFixed = true;
    public Vector3 position;
    public Vector3 direction;
    public Vector3 target;
    public float mass = 1;

    public Vector3 force = Vector3.zero;
    public Vector3 velocity = Vector3.zero;

    public Node(Vector3 position, Vector3 direction, float mass)
    {
        this.position = position;
        this.direction = direction;
        this.mass = mass;
    }

    public void Update(float h, float thickness)
    {
        if (isFixed)
            return;

        force += direction * mass;

        velocity += h * force / mass;

        Vector3 nextPos = position + h * velocity;

        velocity *= 0.99f;

        direction = velocity.normalized;

        if (TestCollision(h, thickness))
            isFixed = true;
        else
            position = nextPos;

        force = Vector3.zero;
    }

    public bool TestCollision(float h, float thickness)
    {
        RaycastHit hit;
        return Physics.Raycast(position, direction, out hit, h + thickness) && !hit.collider.CompareTag("Tattoo");
    }
}
