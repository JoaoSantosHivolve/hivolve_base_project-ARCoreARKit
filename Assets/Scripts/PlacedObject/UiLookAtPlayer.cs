using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiLookAtPlayer : MonoBehaviour
{
    // Update is called once per frame
    private void Update()
    {
        transform.LookAt(Camera.main.transform.position);
        var lea = transform.localEulerAngles;
        lea.z = 0; lea.x = 0;
        transform.localEulerAngles = lea;
    }
}
