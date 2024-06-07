using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExperimentManager : MonoBehaviour
{
    // ===================================================================
    public MainAgent TheAgent;
    CameraControl TheCameraControl;
    TerminalTextManager TheTerminalTextManager;

    // ===========================
    // User Input UI Controls
    public Button ViewSceneButton;
    public Button UserPromptSubmitButton;
    public TMP_InputField UserPromptInputTB;

    // ===================================================================
    private void Awake()
    {
        TheCameraControl = GetComponentInChildren<CameraControl>();
        TheTerminalTextManager = GetComponent<TerminalTextManager>();

    } // Awake
    // ===================================================================
    void Start()
    {
        // Set Up the Narrative Conversation
        UserPromptSubmitButton.onClick.AddListener(() => UserSubmitButtonAction());
        ViewSceneButton.onClick.AddListener(() => ViewSceneButtonAction());

    } // Start
    // ===================================================================
    // Update is called once per frame
    void Update()
    {
        // Check if Quit the Scene
        if (Input.GetKey(KeyCode.Escape))
        {
            Debug.Log("[INFO] Exit Scene by Escape Button");
            Application.Quit();

        }  // Escape or Quit Q Check 

        // ==========================
        // AgentControls 
        if (Input.GetKeyUp(KeyCode.RightArrow)) TheAgent.TurnRight();
        if (Input.GetKeyUp(KeyCode.LeftArrow)) TheAgent.TurnLeft();
        if (Input.GetKeyUp(KeyCode.UpArrow)) TheAgent.SetWalkForward();
        if (Input.GetKeyUp(KeyCode.DownArrow)) TheAgent.SetStopIdle();
        if (Input.GetKeyUp(KeyCode.End))
        {
            Debug.Log("[DEBUG]: END Pressed:  Agent Run Request: ");
            TheAgent.SetRunning();
        }
        // =================================

        if (Input.GetKeyUp(KeyCode.Home))
        {
            Debug.Log("[DEBUG]: Home Pressed:  Agent View Scene Request: ");
            TheAgent.TakeFrontPicture();
        }
        
        /*
        if (Input.GetKeyUp(KeyCode.K))
        {
            Debug.Log("[DEBUG]:K Pressed: Test OpenAI Request: ");
            TheAgent.TestOpenAIRequest();
           
        }

        if (Input.GetKeyUp(KeyCode.P))
        {
            Debug.Log("[DEBUG]:P Pressed: Test Local Python Server: ");
            TheAgent.TestLocalPythonServer();
        }
        */

    }
    // ===================================================================
    void UserSubmitButtonAction()
    {
        // Confirm that the User has entered at least some Text, to avoid Calling with empty Queries
        if (UserPromptInputTB.text.Length < 1) return;

        Debug.Log("[INFO]: User Prompt: " + UserPromptInputTB.text);

        // Send the Remark to the Agent
        TheAgent.UserInteraction(UserPromptInputTB.text);
        UserPromptInputTB.text = "";

    } // UserSubmitButtonAction
    // ===================================================================
    void ViewSceneButtonAction()
    {
        TheAgent.TakeFrontPicture();
        Debug.Log("[INFO]: Agent View Scene Request: ");
    } // ViewSceneButtonAction

    // ===================================================================




    // ===================================================================



    // ===================================================================
}
