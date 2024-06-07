using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using FrostweepGames.Plugins.GoogleCloud.TextToSpeech;

public class MainAgent : MonoBehaviour
{
    // ======================================================================================================
    public enum AgentStates { Idle, WalkingForward, Running,Dying, Attacking,InDeath}

    // ======================================================================================================
    public ExperimentManager TheExperimentManager;
    public TrafficManager TheTrafficManager;
    public TerminalTextManager TheTerminalTextManager;

    public AgentStates TheAgentState;
    Camera TheAgentCamera; 

    // Character Controlled Agent
    private CharacterController TheCharController;
    private float WalkingSpeed = 3.5f;
    private float RunningSpeed = 6.0f;
    private float RotationRate = 20.0f;   // Degrees per second  
    public bool CurrentlyGrounded;
    private Vector3 DeltaLocalMovement;
    private float RequestedDeltaRotation;

    // Animation Management
    Animator TheAnimator;

    // Death Management 
    float DeathPeriod = 15.0f;
    float RecoveryTime;
    public RenderTexture TheAgentViewRenderTexture; 

    // Interface with OpenAI
    OpenAIInterface TheOpenAIInterface;

    float OpenAIResponseCheckPeriod = 1.0f;
    float NextOpenAICheckResponseTime;
    bool OutstandingViewRequest;

    // Text To Speech
    private GCTextToSpeech TheTextToSpeechGenerator;
    private Voice[] PossibleVoices;
    private Voice TheCurrentVoice;
    [SerializeField] AudioSource TheAudioPlayer;

    // ======================================================================================================
    void Awake()
    {
        TheCharController = GetComponent<CharacterController>();
        TheAnimator = GetComponent<Animator>();

        TheAgentCamera = GetComponentInChildren<Camera>();

        RequestedDeltaRotation = 0.0f; 
        CurrentlyGrounded = false;
        OutstandingViewRequest = false;

    } // Awake

    // ======================================================================================================
    // Start is called before the first frame update
    void Start()
    {
        SetStopIdle();

        // Create an OpenAI Interface
        Debug.Log("[INFO]: Agent Attempting to Create an OpenAI Interface"); 
        TheOpenAIInterface = new OpenAIInterface();
        NextOpenAICheckResponseTime = Time.time + OpenAIResponseCheckPeriod;
        RecoveryTime = Time.time;
        // Instantiate Text To Speech
        TheTextToSpeechGenerator = GCTextToSpeech.Instance;

        // Text To Speech Events
        TheTextToSpeechGenerator.GetVoicesSuccessEvent += TheTextToSpeechGenerator_GetVoicesSuccessEvent;
        TheTextToSpeechGenerator.SynthesizeSuccessEvent += TheTextToSpeechGenerator_SynthesizeSuccessEvent;

        TheTextToSpeechGenerator.GetVoicesFailedEvent += TheTextToSpeechGenerator_GetVoicesFailedEvent;
        TheTextToSpeechGenerator.SynthesizeFailedEvent += TheTextToSpeechGenerator_SynthesizeFailedEvent;

        TheTerminalTextManager.AddDisplayedCommand("Hello I am Lucy. What shall we do today? ");
        AgentSpeech("Hello I am Lucy. What shall we do today? ");

    } // Start
      // ======================================================================================================
      // The Text To Speech Voice Responses Events
    private void TheTextToSpeechGenerator_SynthesizeFailedEvent(string arg1, long arg2)
    {
        Debug.Log("[ERROR]: Speech To Synth Text Failure: " + arg1 + "  :  " + arg2.ToString());
    }
    // ====================================
    private void TheTextToSpeechGenerator_GetVoicesFailedEvent(string arg1, long arg2)
    {
        Debug.Log("[ERROR]: Speech To Voices Text Failure: " + arg1 + "  :  " + arg2.ToString());
    }
    // ====================================
    private void TheTextToSpeechGenerator_SynthesizeSuccessEvent(PostSynthesizeResponse response, long arg2)
    {
        Debug.Log("[INFO]: Speech Response received: ...... " );

        TheAudioPlayer.clip = TheTextToSpeechGenerator.GetAudioClipFromBase64(response.audioContent, Constants.DEFAULT_AUDIO_ENCODING);
        TheAudioPlayer.Play();
    }
    // ====================================
    private void TheTextToSpeechGenerator_GetVoicesSuccessEvent(GetVoicesResponse response, long arg2)
    {
        PossibleVoices = response.voices;
    }
    // ====================================
    private void  AgentSpeech(string AgentChatString)
    {
        // Now Set the Doctors Speech
        VoiceConfig TheDoctorsVoiceConfig = new VoiceConfig() { gender = Enumerators.SsmlVoiceGender.FEMALE, languageCode = "en-GB", name = "en-GB-Wavenet-A" };
        // Compile a Synth Voice Request
        TheTextToSpeechGenerator.Synthesize(AgentChatString, TheDoctorsVoiceConfig, true, 1.0, 0.9, Constants.DEFAULT_SAMPLE_RATE, null);
    } // AgentSpeech
    // ===========================================================================

    // =====================================================================================================
    // Update is called once per frame
    void Update()
    {
        



    }  // Update
    // ======================================================================================================

    // Physics Updates
    void FixedUpdate()
    {
        // Check if a Required Rotation has been applied
        this.transform.Rotate(new Vector3(0.0f,RequestedDeltaRotation* RotationRate,0.0f));
        RequestedDeltaRotation = 0.0f;

        // Check Agent Motion
        if ((TheAgentState== AgentStates.WalkingForward) || (TheAgentState == AgentStates.Running))
        {
            // Perform AgentMovement
            UpdateAgentForwardMotion(); 
        }

        // Check If Fallen in River
        if (this.transform.position.y < -0.5f)
        {
            //Debug.Log("[INFO]: Agent Has Fallen in Stream");
            AgentSpeech(" Uh Oh !");
            TheTerminalTextManager.AddSuggestion("Lucy has fallen into stream and drowned");
            UserInteraction("We have just fallen into a Stream and Drowned");
            SetDeath();
        }

        // Check If in Death Period
        if ((TheAgentState == AgentStates.InDeath) && (Time.time > RecoveryTime)) RecoverAgent();

        // ==================================================
        // Periodic Check OpenAI Responses
        if (Time.time>NextOpenAICheckResponseTime)
        {
            // ============
            // Check Plain Text Response
            string OpenAIResponse = "";
            if(TheOpenAIInterface.CheckLastChatResponse(out OpenAIResponse))
            {
                TheTerminalTextManager.AddSuggestion(OpenAIResponse);
                AgentSpeech(OpenAIResponse);
            }
            // ============
            // Check View Response
            string OpenAIViewResponse = "";
            if ((OutstandingViewRequest)&&TheOpenAIInterface.CheckLastViewResponse(out OpenAIViewResponse))
            {
                if (OpenAIViewResponse != "")
                {
                    TheTerminalTextManager.AddViewDescription(OpenAIViewResponse);
                    // Now Set up make a Suggestion What to do Next 
                    TheOpenAIInterface.RequestAnOpenAIViewSuggestion(OpenAIViewResponse);
                }
                OutstandingViewRequest = false;

            } // View Response Received
            // ========================
            NextOpenAICheckResponseTime = Time.time + OpenAIResponseCheckPeriod;
        }
        // =================

    }  // FixedUpdate
    // ======================================================================================================
    public void TurnRight()
    {
        RequestedDeltaRotation = 1.0f;  
       
    } // TurnRight

    public void TurnLeft()
    {
        RequestedDeltaRotation = -1.0f;
    } // Turnleft
    // ==========================================================================================================
    public void TakeFrontPicture() 
    {
        // First Check and return if there is already an Outsanding View Request
        if (OutstandingViewRequest) return;
       
        // Take a Snapshot from the Agent Camera
        // https://forum.unity.com/threads/how-to-save-manually-save-a-png-of-a-camera-view.506269/
        // 
        // ==============================
        TheTerminalTextManager.AddDisplayedCommand(" The Agent is Viewing the Scene.... ");
        // ===============================
        // Need to Encode to ARGB32, to avoid Not too dark Images
        RenderTexture tempRT = new RenderTexture(TheAgentCamera.targetTexture.width, TheAgentCamera.targetTexture.height, 24, RenderTextureFormat.ARGB32)
        {
            antiAliasing = 4
        };

        TheAgentCamera.targetTexture = tempRT;
        RenderTexture.active = tempRT;
        TheAgentCamera.Render();

        Texture2D CapturedImage = new Texture2D(TheAgentCamera.targetTexture.width, TheAgentCamera.targetTexture.height, TextureFormat.ARGB32, false, true);
        CapturedImage.ReadPixels(new Rect(0, 0, CapturedImage.width, CapturedImage.height), 0, 0);
        CapturedImage.Apply();

        RenderTexture.active = null;

        // Now Reset the Agent Camera Target back to the Screen displayed Render texture
        TheAgentCamera.targetTexture = TheAgentViewRenderTexture;

        byte[] bytes = CapturedImage.EncodeToPNG();

        File.WriteAllBytes("D:/AIEnvironmentSnapshots/output.png", bytes);

        // =========================================
        // Now Set Up a View request via OpenAI Interfac e (Via Local Python Server)
      
        // [DEBUG] : Disable Open AI Call whilst revise UI
        TheOpenAIInterface.RequestViewPicture();
        OutstandingViewRequest = true;
        NextOpenAICheckResponseTime = Time.time + OpenAIResponseCheckPeriod;
    } // TakeFrontPicture
    // ========================================================================================================
    void UpdateAgentForwardMotion()
    {
        // Only Move Forwardin Direction currently facing (Unless in Reverse)
        DeltaLocalMovement = transform.forward;

        if (!CurrentlyGrounded)
        {
            DeltaLocalMovement.y = -1000.0f * Time.deltaTime;
        }
        // Ensure that Agent Remains above the Ground Plane
        //if (transform.position.y < -0.2f) DeltaLocalMovement.y = 10.0f * Time.deltaTime;

        // Perform the Movement
        if(TheAgentState == AgentStates.Running) DeltaLocalMovement = DeltaLocalMovement * RunningSpeed * Time.deltaTime;
        if (TheAgentState == AgentStates.WalkingForward) DeltaLocalMovement = DeltaLocalMovement * WalkingSpeed * Time.deltaTime;
        TheCharController.Move(DeltaLocalMovement);
    } // UpdateLeoDirectionandMotion
    // ======================================================================================
    public void UserInteraction(string UserComment)
    {
        TheOpenAIInterface.RequestAnOpenAIChatResponse(UserComment);
        NextOpenAICheckResponseTime = Time.time + OpenAIResponseCheckPeriod;
        OutstandingViewRequest = false;
    } // UserInteraction
    // ======================================================================
    public void AgentEventInteraction(string EventMessage)
    {
        TheOpenAIInterface.SendOpenAIAgentEventMessage(EventMessage);   
        NextOpenAICheckResponseTime = Time.time + OpenAIResponseCheckPeriod;
        OutstandingViewRequest = false;
    } // // AgentEvent

    // =======================================================================================
    public void TestOpenAIRequest() 
    {
        // DEBUG: Disabel Open AI requests whilst revise UI 
        /*
         * 
        // Simple Request to Open AI
        string RequestString = "I see a Church and a building.  What should I do next ?";
        TheTerminalTextManager.AddDisplayedCommand("Agent Request: " + RequestString);
        TheOpenAIInterface.RequestAnOpenAIChatResponse(RequestString);
        */


    }  // TestOpenAIRequest
    // =======================================================================================
    public void TestLocalPythonServer()
    {
        TheTerminalTextManager.AddDisplayedCommand("Local Web Check: " + TheOpenAIInterface.SimpleConnectCheck());
    }  // TestOpenAIRequest

    // ================================================================================
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Car")
        {
            Debug.Log("[INFO]: Agent Has Been Hit By a Car");
            AgentSpeech(" Uh Oh !");
            //AgentEventInteraction("You Have been hit by a car and killed");
            TheTerminalTextManager.AddSuggestion("Lucy has been hit by a Car and Killed"); 
            UserInteraction("We have just been hit by a car and killed");
            SetDeath();
        }

        if (other.gameObject.tag == "ZebraCrossing")
        {
            //Debug.Log("[INFO]: Agent Has Entered Zebra Crossing");
            TheTrafficManager.StopAllCars(); 
        }

    } // OnTriggerEnter
   // =====================================
    private void OnTriggerExit(Collider other)
    {
      
        if (other.gameObject.tag == "ZebraCrossing")
        {
            //Debug.Log("[INFO]: Agent Has Left Zebra Crossing");
            TheTrafficManager.RestartAllCars();
        }

    } // OnTriggerEnter
    // ======================================================================================



    // ======================================================================================
    public void SetWalkForward()
    {
        TheAgentState = AgentStates.WalkingForward;
        TheAnimator.SetTrigger("SetWalking");

    }  // SetWalkForward
    // =============================
    public void SetRunning()
    {
        TheAgentState = AgentStates.Running;
        TheAnimator.SetTrigger("SetRunning");

    } // SetRunning
    // ===============================
    public void SetStopIdle()
    {
        TheAgentState = AgentStates.Idle;
        TheAnimator.SetTrigger("SetIdle");

    } // SetStopIdle
    // =================================
    void SetDeath()
    {
        TheAgentState = AgentStates.InDeath; 
        TheCharController.enabled = false;
        this.transform.position = new Vector3(-118.0f,9.0f,65.0f);
        this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        RecoveryTime = Time.time + DeathPeriod;

    } // SetDeath


    // ======================================================================================================
    void RecoverAgent()
    {
        this.transform.position = new Vector3(33.0f, 0.1f, 11.0f);

        SetStopIdle();
        TheCharController.enabled = true;
        TheTrafficManager.RestartAllCars();
    } // RecoverAgent




    // ======================================================================================================
}
