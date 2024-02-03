using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraMover : MonoBehaviour
{
    private CinemachineTrackedDolly cam;
    private int points;
    [SerializeField,Range(0,1)] private float speed;
    private bool isRunning = true;
    private void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTrackedDolly>();
        points = FindObjectOfType<CinemachineSmoothPath>().m_Waypoints.Length;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) isRunning = !isRunning;
        if (!isRunning) return;
        cam.m_PathPosition += speed * Time.deltaTime;
        if (cam.m_PathPosition >= points) cam.m_PathPosition -= points;
    }
}
