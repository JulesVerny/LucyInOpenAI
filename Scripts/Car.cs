using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    // =================================================================================
    public enum CarStates { Idle,Moving, Stopped }

    public CarStates TheCarState;

    private CharacterController TheCharController;
    private float CarSpeed = 8.0f;
    private float RotationRate = 40.0f;   // Degrees per second  
    public bool CurrentlyGrounded;
    private Vector3 DeltaLocalMovement;

    // Local WayPoints Management
    private List<Vector3> WayPoints;
    public int NextWayPoint = 0;
    private float WayPointThreshold2 = 2.0f;  // WayPoint Threshold
    float NextWaypointX, NextWaypointZ, DistanceToNextWayPoint2;
    Quaternion RequiredRotation;

    // =============================================================================
   
    void Awake()
    {
        TheCharController = GetComponent<CharacterController>();
       
        CurrentlyGrounded = false;

        WayPoints = new List<Vector3>();
        WayPoints.Clear();
        NextWayPoint = 0;

        // Set Up Road WayPoints
        WayPoints.Add(new Vector3(-83.0f, 0.0f, -14.0f));
        WayPoints.Add(new Vector3(-64.0f, 0.0f, 9.0f));
        WayPoints.Add(new Vector3(-43.5f, 0.0f, 46.5f));
        WayPoints.Add(new Vector3(-16.0f, 0.0f, 53.5f));
        WayPoints.Add(new Vector3(3.0f, 0.0f, 53.0f));
        WayPoints.Add(new Vector3(23.0f, 0.0f, 57.0f));
        WayPoints.Add(new Vector3(56.4f, 0.0f, 73.0f));
        WayPoints.Add(new Vector3(104.4f, 0.0f, 73.0f));
        WayPoints.Add(new Vector3(138.0f, 0.0f, 75.0f));
        WayPoints.Add(new Vector3(154.0f, 0.0f, 85.0f));

        TheCarState = CarStates.Moving;

    } // Awake

    // ======================================================================================================


    // ======================================================================================================
    void Start()
    {
        



    }

    // Update is called once per frame
    void Update()
    {
        


    }

    // =============================================================================
    // Physics Updates
    void FixedUpdate()
    {

        // Waypoint Monitoring if Walking
        if ((TheCarState == CarStates.Moving) && (WayPoints.Count > 0) && (NextWayPoint < WayPoints.Count))
        {
            NextWaypointX = WayPoints[NextWayPoint].x;
            NextWaypointZ = WayPoints[NextWayPoint].z;
            DistanceToNextWayPoint2 = ((NextWaypointX - transform.position.x) * (NextWaypointX - transform.position.x) + (NextWaypointZ - transform.position.z) * (NextWaypointZ - transform.position.z));

            if (DistanceToNextWayPoint2 < WayPointThreshold2)
            {
                // Progress Towards Next WayPoint
                NextWayPoint++;
                // Now Check If Reached Last WayPoint => Destroy Car
                if (NextWayPoint == WayPoints.Count) Destroy(this.gameObject);
            }  // Reached WayPoint

            if(this.transform.position.x > 120.0f) Destroy(this.gameObject);

            // Update Zombie Motion and Direction
            if (NextWayPoint < WayPoints.Count) UpdateCarMotion();
        } //  WayPoint Monitoring

    }  // FixedUpdate
    // ======================================================================================================
    void UpdateCarMotion()
    {
        // Perform the Rotation Towards the Next WayPoint at Rotation rate
        RequiredRotation = Quaternion.LookRotation(WayPoints[NextWayPoint] - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, RequiredRotation, RotationRate * Time.deltaTime);

        // Always and Only Move in Direction currently facing (Unless in Reverse)
        DeltaLocalMovement = transform.forward;

        if (!CurrentlyGrounded)
        {
            DeltaLocalMovement.y = -1000.0f * Time.deltaTime;
        }

        // Perform the Movement
        if (TheCarState == CarStates.Moving) DeltaLocalMovement = DeltaLocalMovement * CarSpeed * Time.deltaTime;
        TheCharController.Move(DeltaLocalMovement);

    } // UpdateCarMotion
    // ======================================================================================
    public void StopCar()
    {
        TheCarState = CarStates.Stopped;
    }
    public void RestartCar()
    {
        TheCarState = CarStates.Moving;
    }
    // =============================================================================




    // =============================================================================
    void OnCollisionEnter(Collision theCollision)
    {
        // Check On Ground
        if (theCollision.gameObject.name == "GroundTerrain") CurrentlyGrounded = true;
    } // OnCollisionEnter
    // =======================
    void OnCollisionExit(Collision theCollision)
    {
        // Check left Ground (e.g. deliberate Jump !)
        if (theCollision.gameObject.name == "GroundTerrain") CurrentlyGrounded = false;
    } // OnCollisionExit
    // ===========================================================================================

    // =============================================================================
}
