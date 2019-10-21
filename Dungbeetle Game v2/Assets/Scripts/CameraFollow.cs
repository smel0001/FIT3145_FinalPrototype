using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public float y_offset = 0.0f;

    public GameObject player;

    void LateUpdate()
    {
        Vector3 pos = player.transform.position;
        pos.z = -10f;
        pos.y = pos.y + y_offset;
        transform.position = pos;
    }
}
