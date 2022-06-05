using System;
using UnityEngine;

public class CameraSizeFix : MonoBehaviour
{
    void Update()
    {
        GetComponent<Camera>().orthographicSize = 3.5f / (float)Math.Sqrt((float)Screen.width / (float)Screen.height);
    }
}
