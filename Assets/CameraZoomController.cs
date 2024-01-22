using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraZoomControler : MonoBehaviour
{

    CinemachineFreeLook vcam;

    // Start is called before the first frame update
    void Start()
    {   
        vcam = GetComponent<CinemachineFreeLook>();
    }

    // Update is called once per frame
    void OnPreRender()
    {
        vcam.m_Lens.FieldOfView = World.Instance.cameraFOV;
    }
}
