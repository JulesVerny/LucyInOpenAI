using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DalexControl : MonoBehaviour
{
    private CharacterController TheCharController;
    private float WalkingSpeed = 0.75f;
    private float RotationRate = 20.0f;   // Degrees per second  
    public bool CurrentlyGrounded;
    private Vector3 DeltaLocalMovement;


    // Local WayPoints Management
    private List<Vector3> WayPoints;
    private int NextWayPoint = 0;
    private float WayPointThreshold2 = 0.5f;  // WayPoint Threshold
    float NextWaypointX, NextWaypointZ, DistanceToNextWayPoint2;
    Quaternion RequiredRotation;
    float RandomWalkDistance = 7.5f;

    // ====================================================================================
    void Awake()
    {
        TheCharController = GetComponent<CharacterController>();
      
        CurrentlyGrounded = false;

        WayPoints = new List<Vector3>();
        WayPoints.Clear();
        NextWayPoint = 0;

    } // Awake

    // ====================================================================================
    // Start is called before the first frame update
    void Start()
    {
        SetLocalRoute();
    }
    // ====================================================================================

    // Update is called once per frame
    void Update()
    {
        
    }
    // ====================================================================================
    // Physics Updates
    void FixedUpdate()
    {

        // Waypoint Monitoring if Walking
        if ((WayPoints.Count > 0) && (NextWayPoint < WayPoints.Count))
        {
            NextWaypointX = WayPoints[NextWayPoint].x;
            NextWaypointZ = WayPoints[NextWayPoint].z;
            DistanceToNextWayPoint2 = ((NextWaypointX - transform.position.x) * (NextWaypointX - transform.position.x) + (NextWaypointZ - transform.position.z) * (NextWaypointZ - transform.position.z));

            if (DistanceToNextWayPoint2 < WayPointThreshold2)
            {
                // Progress Towards Next WayPoint
                NextWayPoint++;
                // Now Check If Reached Destination
                if (NextWayPoint == WayPoints.Count) NextWayPoint = 0;
            }  // Reached WayPoint

            // Update Zombie Motion and Direction
            if (NextWayPoint < WayPoints.Count) UpdateDalekMotion();
        } //  WayPoint Monitoring

    }  // FixedUpdate
    // ======================================================================================================

    void UpdateDalekMotion()
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
        DeltaLocalMovement = DeltaLocalMovement * WalkingSpeed * Time.deltaTime;
        TheCharController.Move(DeltaLocalMovement);

    } // UpdateLeoDirectionandMotion
    // ======================================================================================
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



    // ======================================================================================
    void SetLocalRoute()
    {
        // Set up a few Waypoints in Local 
        WayPoints.Clear();
        NextWayPoint = 0;

        for (int i = 0; i < 5; i++)
        {
            Vector3 RandomDirection = Random.insideUnitSphere * RandomWalkDistance;
            WayPoints.Add(new Vector3(this.transform.position.x + RandomDirection.x, this.transform.position.y, this.transform.position.z + RandomDirection.z));
        }

    } // SetLocalRoute
    // ======================================================================================


    // ====================================================================================
}
