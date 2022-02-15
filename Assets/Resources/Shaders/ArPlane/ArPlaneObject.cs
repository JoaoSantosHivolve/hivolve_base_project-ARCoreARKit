using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArPlaneObject : MonoBehaviour
{
    public Renderer plane;

    private void Update()
    {
        plane.material.SetVector("_Position", transform.position);
    }
}
