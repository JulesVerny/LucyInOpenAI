using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
    //
    float DeathPeriod = 5.0f;
    float RecoveryTime;

    // ===============================================
    void Start()
    {
        RecoveryTime = Time.time + DeathPeriod; 

    }
    // ============================================================================
    // Update is called once per frame
    void Update()
    {

        if(Time.time>RecoveryTime) 
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);

        }
        
    }

    // ============================================================================



    // ============================================================================

}
