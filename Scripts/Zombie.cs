using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public enum ZombieStates { Idle, Walking,Running,  Dying, Attacking }

    // ======================================================================================================
    public ZombieStates TheZombieState;

    // Character Controlled Agent
    private CharacterController TheCharController;
    private float WalkingSpeed = 0.5f;
    private float RunningSpeed = 2.5f;
    private float RotationRate = 20.0f;   // Degrees per second  
    public bool CurrentlyGrounded;
    private Vector3 DeltaLocalMovement;
    

    // Local WayPoints Management
    private List<Vector3> WayPoints;
    private int NextWayPoint = 0;
    private float WayPointThreshold2 = 0.5f;  // WayPoint Threshold
    float NextWaypointX, NextWaypointZ, DistanceToNextWayPoint2;
    Quaternion RequiredRotation;
    float RandomWalkDistance = 10.0f; 


    // Animation Management
    Animator TheAnimator;

    // ======================================================================================================
    void Awake()
    {
        TheCharController = GetComponent<CharacterController>();
        TheAnimator = GetComponent<Animator>();

        CurrentlyGrounded = false;

        WayPoints = new List<Vector3>();
        WayPoints.Clear();
        NextWayPoint = 0;

    } // Awake

    // ======================================================================================================
    // Start is called before the first frame update
    void Start()
    {
        SetLocalRoute();

    }
    // ======================================================================================================

    // Update is called once per frame
    void Update()
    {




    }  // Update
    // ======================================================================================================

    // Physics Updates
    void FixedUpdate()
    {

        // Waypoint Monitoring if Walking
        if ((TheZombieState == ZombieStates.Walking)&& (WayPoints.Count > 0) && (NextWayPoint < WayPoints.Count))
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
            if (NextWayPoint < WayPoints.Count) UpdateZombieMotion();
        } //  WayPoint Monitoring
   
        // Now Monitor if in Vicinity of the Player

        // TODO 


    }  // FixedUpdate
    // ======================================================================================================
   


    // ========================================================================================================
    void UpdateZombieMotion()
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
        if (TheZombieState == ZombieStates.Walking) DeltaLocalMovement = DeltaLocalMovement * WalkingSpeed * Time.deltaTime;
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
            Vector3 RandomDirection = Random.insideUnitSphere* RandomWalkDistance;
            WayPoints.Add(new Vector3(this.transform.position.x + RandomDirection.x, this.transform.position.y, this.transform.position.z + RandomDirection.z));
        }

        SetWalking();

    } // SetLocalRoute
    // ======================================================================================
    public void SetWalking()
    {
        TheZombieState = ZombieStates.Walking;
        TheAnimator.SetTrigger("Walking");

    }  // SetWalkForward
    // =============================
    public void SetRunning()
    {
        TheZombieState = ZombieStates.Running;
        TheAnimator.SetTrigger("Running");

    }  // SetRunning
    // =============================
    public void SetAttacking()
    {
        TheZombieState = ZombieStates.Attacking;
        TheAnimator.SetTrigger("Attack");

    } // SetAttacking
    // ===============================
    public void SetStopIdle()
    {
        TheZombieState = ZombieStates.Idle;
        TheAnimator.SetTrigger("StopIdle");

    } // SetStopIdle
    // =================================
   
    public void SetDying()
    {
        TheZombieState = ZombieStates.Dying;
        TheAnimator.SetTrigger("Death");

    } // SetStopIdle
    // =================================


    // ======================================================================================================




}
