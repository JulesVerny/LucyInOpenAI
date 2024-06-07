using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{
    // =======================================================
    public GameObject Car1Prefab;
    public GameObject Car2Prefab;
    public GameObject Car3Prefab;

    // =======================
    int NextCarType;
    float CarGenerationPeriod = 5.0f;
    float NextCarGenerationTime;

    Vector3 StartLocation;
    Quaternion StartRotation;

    // =======================================================

    // ====================================================
    // Start is called before the first frame update
    void Start()
    {
        NextCarType = 1;
        StartLocation = new Vector3(-120.0f, 1.0f, -16.0f);
        StartRotation = Quaternion.Euler(0.0f, 100.0f, 0.0f); 

        NextCarGenerationTime = Time.time + CarGenerationPeriod;


    }
    // ==============================================================================
    // Update is called once per frame
    void Update()
    {
        


    }

    // ================================================================================
    private void FixedUpdate()
    {
        // =================================
        // First Check New Car Generation Time
        if (Time.time >= NextCarGenerationTime)
        {
            // Instnatiate the Next Car
            if (NextCarType == 1) Instantiate(Car1Prefab, StartLocation, StartRotation);
            if (NextCarType == 2) Instantiate(Car2Prefab, StartLocation, StartRotation);
            if (NextCarType == 3) Instantiate(Car3Prefab, StartLocation, StartRotation);

            NextCarType++;
            if (NextCarType == 4) NextCarType = 1; 
            NextCarGenerationTime = Time.time + CarGenerationPeriod;
        }
        // =================================



        // =================================
        //  Review All Cars
        // TODO


        // =================================

    } // FixedUpdate
    // ================================================================================
    public void StopAllCars()
    {
        GameObject[] AllCars = GameObject.FindGameObjectsWithTag("Car");
        foreach (GameObject ACarGO in AllCars)
        {
            Car CarScript = ACarGO.GetComponent<Car>();
            if (CarScript != null) CarScript.StopCar();
        }
        
        // Stop Generating Cars
        NextCarGenerationTime = Time.time + 100*CarGenerationPeriod;

    } // StopAllCars
    // ====================================
    public void RestartAllCars()
    {
        GameObject[] AllCars = GameObject.FindGameObjectsWithTag("Car");
        foreach (GameObject ACarGO in AllCars)
        {
            Car CarScript = ACarGO.GetComponent<Car>();
            if (CarScript != null) CarScript.RestartCar();
           
        }
        NextCarGenerationTime = Time.time + CarGenerationPeriod;
    } // RestartAllCars
    // ================================================================================



    // ================================================================================
}
