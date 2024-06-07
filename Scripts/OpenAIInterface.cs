using System.Collections;
using System.Collections.Generic;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using static OpenAI_API.Chat.ChatMessage;
using OpenAI_API.Completions;
using OpenAI_API.Moderation;
using System.IO; 
using System;
using System.Net;
using UnityEngine;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using UnityEngine.PlayerLoop;

// ===============================================
[System.Serializable]
public class SimpleWebResponseInfo
{
    public string result;
}
[System.Serializable]
public class DescriptionWebResponseInfo
{
    public string result;
    public string description;
}
[System.Serializable]
public class SimpleWebRequestInfo
{
    public string request;
}
// ===========================================
public class OpenAIInterface
{
    // ================================================
    // Interface with the OpenAI API
    private OpenAIAPI TheOpenAI_api;
    private List<ChatMessage> AgentMessageHistory;
    private int ResponseCount;
    private int LastCheckCount;
    private string LastAPIMessageResponse;

    // Local Web Service Reference
    HttpClient LocalPythonHttpClient; 

    // ======================================================================================
    public OpenAIInterface()
    {
        // Instantiate the Open AI Interface
        TheOpenAI_api = new OpenAIAPI(Environment.GetEnvironmentVariable("OPEN_AI_KEY", EnvironmentVariableTarget.Machine));
        // Initialise the Message History for a new Agent Conversation Session 
        AgentMessageHistory = new List<ChatMessage>();

        // Now Set up Initial System Context
        ChatMessage AgentIntroductoryMessage = new ChatMessage(ChatMessageRole.System, "You are acting as a guide. You will be provided with list of items in view. You are then to make suggestions to the user on what to do next, in response to items that you have seen. Please provide helpful suggestions on what the user could do next and explore.  However please advise to avoid any obvious hazards. Please ensure that your responses are brief and succint.");
        AgentMessageHistory.Add(AgentIntroductoryMessage);

        ResponseCount = 0;
        LastCheckCount = ResponseCount;
        LastAPIMessageResponse = "";

        // Set Up the Local Python Http Client
        LocalPythonHttpClient = new HttpClient();
        LocalPythonHttpClient.BaseAddress = new Uri("http://127.0.0.1:7070/");

    } // OpenAIInterface
    // ======================================================================================
    public async void RequestAnOpenAIChatResponse(string ReqMessage)
    {
        // Now Retrieve the User Message from the Narrator Input
        ChatMessage AgentChatRequestMessage = new ChatMessage(ChatMessageRole.User, ReqMessage);
        AgentMessageHistory.Add(AgentChatRequestMessage);

        // Set Up the Chat Request Object
        ChatRequest TheChatRequest = new ChatRequest()
        {
            Model = "gpt-4o", //Model.ChatGPTTurbo,
            Temperature = 0.8,
            Messages = AgentMessageHistory 
        };

        // Now Make the Chat Call into Open API Interface
        var APIChatResult = await TheOpenAI_api.Chat.CreateChatCompletionAsync(TheChatRequest);

        // Now process the API Result after receiving the response
        ChatMessage APIResponseMessage = new ChatMessage();
        APIResponseMessage.Role = APIChatResult.Choices[0].Message.Role;
        APIResponseMessage.Content = APIChatResult.Choices[0].Message.Content;

        // Ensure that Add the API Response to the Agent message History
        AgentMessageHistory.Add(APIResponseMessage);

        // TODO Need to Incorporate into Unity AI Environment variables
        LastAPIMessageResponse = APIChatResult.Choices[0].Message.Content; 
        ResponseCount++;

    } // RequestChatResponse
    // =======================================================================================
    public async void SendOpenAIAgentEventMessage(string EventMessage)
    {
        // Now Retrieve the User Message from the Narrator Input
        ChatMessage AgentChatEventMessage = new ChatMessage(ChatMessageRole.System, EventMessage);
        AgentMessageHistory.Add(AgentChatEventMessage);

        // Set Up the Chat Request Object
        ChatRequest TheChatRequest = new ChatRequest()
        {
            Model = "gpt-4o", 
            Temperature = 0.8,
            Messages = AgentMessageHistory
        };

        // Now Make the Chat Call into Open API Interface
        var APIChatResult = await TheOpenAI_api.Chat.CreateChatCompletionAsync(TheChatRequest);

        // Now process the API Result after receiving the response
        ChatMessage APIResponseMessage = new ChatMessage();
        APIResponseMessage.Role = APIChatResult.Choices[0].Message.Role;
        APIResponseMessage.Content = APIChatResult.Choices[0].Message.Content;

        // Ensure that Add the API Response to the Agent message History
        AgentMessageHistory.Add(APIResponseMessage);

        LastAPIMessageResponse = APIChatResult.Choices[0].Message.Content;
        ResponseCount++;

    } // RequestChatResponse
    // =======================================================================================
    public async void RequestAnOpenAIViewSuggestion(string ViewMessage)
    {
        // Compile a Sugegstion Request [Assitant + User Messages] 
        string AgentViewUpdate = "You can see the following items: " + ViewMessage;

        ChatMessage AgentChatRequestMessage = new ChatMessage(ChatMessageRole.System, AgentViewUpdate);
        AgentMessageHistory.Add(AgentChatRequestMessage);

        ChatMessage UserRequestMessage = new ChatMessage(ChatMessageRole.User, "What do you suggest I do next in response to what you have just seen?");
        AgentMessageHistory.Add(UserRequestMessage);

        // Set Up the Chat Request Object
        ChatRequest TheChatRequest = new ChatRequest()
        {
            Model = "gpt-4o", 
            Temperature = 0.8,
            Messages = AgentMessageHistory  
        };

        // Now Make the Chat Call into Open API Interface
        var APIChatResult = await TheOpenAI_api.Chat.CreateChatCompletionAsync(TheChatRequest);

        // Now process the API Result after receiving the response
        ChatMessage APIResponseMessage = new ChatMessage();
        APIResponseMessage.Role = APIChatResult.Choices[0].Message.Role;
        APIResponseMessage.Content = APIChatResult.Choices[0].Message.Content;

        // Ensure that Add the API Response to the Agent message History
        AgentMessageHistory.Add(APIResponseMessage);

        // TODO Need to Incorporate into Unity AI Environment variables
        LastAPIMessageResponse = APIChatResult.Choices[0].Message.Content;
        ResponseCount++;

    } // RequestChatResponse
    // =======================================================================================
    public bool CheckLastChatResponse(out string LastResponse)
    {
        if (LastCheckCount == ResponseCount)
        {
            // No Change Since Last Call
            LastResponse = "";
            return false;
        }
        // Otherwise We have received a response

        LastCheckCount = ResponseCount;
        LastResponse = LastAPIMessageResponse;
        return true;
    } // CheckLastResponse
    // =======================================================================================
    //  Web Service Requests
    public string SimpleConnectCheck()
    {
        // Just Perforn a simple GET Request 
        System.Uri UnitySimpleWebServerURL = new System.Uri("http://127.0.0.1:7070/");   // The Plain Python GET Home Page Response 
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UnitySimpleWebServerURL);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string WebTextResponse = reader.ReadToEnd();

        return(WebTextResponse);

    }  // SimpleConnectCheck
    // ===========================================================================================
    
    // =========================================================================================
    public void RequestViewPicture()
    {
        // n.n don't need to await because not waiting for the result here
         PostViewPictureAsync(LocalPythonHttpClient);

    } // RequestViewPicture
    // =========================================================================================
    public static async Task PostViewPictureAsync(HttpClient LocalHttpClient)
    {
        // An Async POST request to the Local Python Server 
        SimpleWebRequestInfo SimpleRequest = new SimpleWebRequestInfo();
        SimpleRequest.request = "vision";

        using StringContent SimpleJsonRequest = new(JsonUtility.ToJson(SimpleRequest),Encoding.UTF8,"application/json");

        using HttpResponseMessage httpResponse = await LocalHttpClient.PostAsync("requestview", SimpleJsonRequest);

        string RawMessageContent = await httpResponse.Content.ReadAsStringAsync();
        SimpleWebResponseInfo ResultInfo = JsonUtility.FromJson<SimpleWebResponseInfo>(RawMessageContent);

        Debug.Log("[INFO]: Local View Request: " + ResultInfo.result);

    } // PostViewPictureAsync
    // =========================================================================================



    // ================================================================================================
    public bool CheckLastViewResponse(out string TheViewDescription)
    { 
        // Set Up Request via Local Python Server
        System.Uri UnityPOSTPosUpdateURL = new System.Uri("http://127.0.0.1:7070/QueryPictureRequest");   // The Python POST process URI 

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UnityPOSTPosUpdateURL);
        request.Method = "POST";
        request.ContentType = "application/json";

        // Now Send the Request & Process the Response
        var httpResponse = (HttpWebResponse)request.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
            var ResponseText = streamReader.ReadToEnd();
            DescriptionWebResponseInfo ResultInfo = JsonUtility.FromJson<DescriptionWebResponseInfo>(ResponseText);
            if(ResultInfo.result == "Success")
            {
                Debug.Log("[INFO]: View Description Received: " + ResultInfo.description);
                TheViewDescription = ResultInfo.description;
                return true;
            }
            else
            {
                TheViewDescription = ""; 
                return false;
            }
        }

    } // CheckLastViewResponse
    // =========================================================================================

}
