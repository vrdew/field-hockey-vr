using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorMovement : MonoBehaviour
{
    public Transform playerTarget;
    public Transform Mirror;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 localPlayer = Mirror.InverseTransformPoint(playerTarget.position);
        transform.position = Mirror.TransformPoint(new Vector3(localPlayer.x, localPlayer.y, -localPlayer.z));

        Vector3 lookatmirror = Mirror.TransformPoint(new Vector3(-localPlayer.x, localPlayer.y, localPlayer.z));
        transform.LookAt(lookatmirror);
    }
}
