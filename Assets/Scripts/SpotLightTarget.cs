using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightTarget : MonoBehaviour
{
    [SerializeField]
    bool validate;
    [SerializeField]
    Transform target;
    [SerializeField]
    float distance = 5f;

    // Update is called once per frame
    void FixedUpdate()
    {
        Adjust();
    }

    private void OnDrawGizmos()
    {
        Adjust();
    }

    void Adjust()
    {
        if (GetComponent<Light>().type == LightType.Spot && target)
        {
            transform.LookAt(target);
            transform.position = target.transform.position - (distance * transform.forward);
        }
    }
}
