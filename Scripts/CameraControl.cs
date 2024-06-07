using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{

    public GameObject TargetObject;

    public float RotationFactor = 12.5f;
    public float SmoothFactor = 0.25f;

    private Vector3 CameraOffset;
    private Vector3 SmoothedTargetPosition; 

    // ======================================================================================
   
    // ======================================================================================
    void Start()
    {
        // Set Camera Up and Behind Target GameObject
        transform.position = TargetObject.transform.position - TargetObject.transform.forward * 25.0f + Vector3.up * 10.0f;
        transform.LookAt(TargetObject.transform); 

        SmoothedTargetPosition = TargetObject.transform.position + Vector3.up * 1.5f;
        CameraOffset = transform.position - SmoothedTargetPosition;
    } // Start
    // ======================================================================================
    // Update is called once per frame
    void Update()
    {
        
        SmoothedTargetPosition = 0.99f * SmoothedTargetPosition + 0.01f * (TargetObject.transform.position + Vector3.up * 1.5f);
        Vector3 NewCameraPosition = SmoothedTargetPosition + CameraOffset;
        transform.position = Vector3.Slerp(transform.position, NewCameraPosition, SmoothFactor);
        transform.LookAt(TargetObject.transform);

    } // Update
    // ======================================================================================

    public void CameraZoomOutButtonPressed()
    {
        if (CameraOffset.magnitude <17.5f) CameraOffset = CameraOffset * 1.25f;

    } // CameraZoomOutButtonPressed
    // ====================================
    public void CameraZoomInButtonPressed()
    {
        if(CameraOffset.magnitude>2.5f) CameraOffset = CameraOffset * 0.75f;

    } // CameraZoomInButtonPressed
    // ====================================
    public void CameraPanRightButtonPressed()
    {    
        Quaternion CameraTurnAngle = Quaternion.AngleAxis(RotationFactor, Vector3.up);
        CameraOffset = CameraTurnAngle * CameraOffset;

    } // CameraPanRightButtonPressed
    // ====================================
    public void CameraPanLeftButtonPressed()
    {  
        Quaternion CameraTurnAngle = Quaternion.AngleAxis(-RotationFactor, Vector3.up);
        CameraOffset = CameraTurnAngle * CameraOffset;

    } // CameraPanLeftButtonPressed
    // ======================================================================================
}
