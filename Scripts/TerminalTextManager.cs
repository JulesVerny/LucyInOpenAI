using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using System.Text;

public class TerminalTextManager : MonoBehaviour
{
    // =========================================================================================
    public TMP_Text TerminalDisplayTB;

    private List<string> CommandList;
    private StringBuilder FullTerminalTextSB;

    // =========================================================================================
    private void Awake()
    {
        CommandList = new List<string>();
    }
    // ===================================================================================
    // Start is called before the first frame update
    void Start()
    {
        UpdateDisplayedText();
    }
    // =========================================================================================
    // Update is called once per frame
    void Update()
    {
        TerminalDisplayTB.text = FullTerminalTextSB.ToString();
    }
    // =========================================================================================
    void UpdateDisplayedText()
    {
        // Display the Terminal List
        FullTerminalTextSB = new StringBuilder();

        foreach (string DisplCommand in CommandList)
        {
            FullTerminalTextSB.Append(DisplCommand);
            FullTerminalTextSB.Append("\n");
        }
    } // UpdateDisplayedText
    // =========================================================================================
    public void AddDisplayedCommand(string NewDisplayedCommand)
    {
        if(CommandList.Count>=18)
        {
            // Remove the First Item in the list
            CommandList.RemoveAt(0);
        }
        CommandList.Add((string)NewDisplayedCommand);

        UpdateDisplayedText();

    } // AddDisplayedCommand
    // =========================================================================================
    public void AddViewDescription(string ViewDescription)
    {
        CommandList.Clear();
        CommandList.Add("I Can See the Following features: ");
        CommandList.Add((string)ViewDescription);

        UpdateDisplayedText();

    } // AddDisplayedCommand
    // =======================================================================
    public void AddSuggestion(string SuggestionText)
    {
        CommandList.Clear();
        CommandList.Add((string)SuggestionText);

        UpdateDisplayedText();

    } // AddDisplayedCommand

    // =========================================================================================
}
