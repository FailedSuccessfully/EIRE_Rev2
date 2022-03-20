using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    CinemachineVirtualCamera cam;
    CinemachineTrackedDolly dol;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
        dol = cam.GetCinemachineComponent<CinemachineTrackedDolly>();
        dol.m_PathPosition = 0;
    }

    // Update is called once per frame
    void Update()
    {
        dol.m_PathPosition += Time.deltaTime * speed;
    }
}
