# LucyInOpenAI
An Experiment using OpenAI Vision and Chat API Services from a Unity Game Environment 

Some goofing around with the OpenAI GPT4o Chat and Vision API, from a Unity Environment .
I have also used the Google Text to Speech API service to animate the Agnet oices, of the returned Chat textual conversations. 

![ScreenShot](OverviewPic2.PNG)

## Video Demonstration ##
Please see the [brief You Tube Video demonstration here](https://www.youtube.com/watch?v=rAbuMg2NdRY)  


## Experiment Architecture ##


Note the Agent is Manually controlled, with Manual initiated requests for a vision cature and awareness. There is no automated Agent control (yet) in this simulation.  

The C# OKGoDoIt wrapper servcies for OpenAI, are awaiting a major revamp. See his Github page, to utilise the latest OpenAI services. I had difficulty in getting the current provided C# wrapper API, to work with the Open AI GPT4o vision (image upload) models. So being impatient, I reverted to a pyhton intercation with Open AI for the GPT4o vision API. There is a lot quicker and more reponsive support in python OpneAi packages. So I deffered those image interaction into a local python flask web server. This picked up the local image file, and uploaded it into OpenAI GPT4o visions services.  The Unity environment, then query polls the web service for any responses      


##  Open AI Services ##


## Unity Scene  ##




### Unity Package Limitation ###



![ScreenShot](Warning.png)

There are four Unity script files:
- ExperimentManager.CS      :  This is the overall Experiment coordination. It manages the responses from the User Interface, into the Agent
- MainAgent.CS         :  This Script controls the Agent within the Environment. It coordinates the user requested Agent movements, and situational awareness Agent requests. 
- OpenAIInterface.CS     :  This provides a wrapper around the calls to the OpenAI services. It makes direct calls to OpenAI GPT4o Chat, but Indirect Vision requests via local Web Interface. ( Note many of these methods have to be implemented as async, to avoid the slow web services reponses halting the main Agent response thread. )   
-   TerminalManager.CS   :  This manages a Terminal like Usr Interface. 


Note I have provided a most of the Unity Enviornment here, as a Unity Package.  However some of the third party assets are missing due to size and lisence: 
- Zombie Models (Note these are a free download from Mizamo)      
- FrostByte Text To Speech Services (Paid Asset in Unity Asset Store)  
- Jodrell Bank and the StormTrooper models
- SkyBox Materials and Textures (Free Asset from Unity Asset Store) 
- A few Materials Textures, as too large to export  


## Discussion ##

The experience is quite fun, buit a little stilted. The Web based OpenAI Reques/Responses take several seconds and so it is not a real time responsive experience. OpenAI Vsion is only accepting still images, and not live real time video. And so there is a latency and delay from what was viwed previously.  

The OpenAI vion can only describe main objects and features within the View presented to it. It cannot appreciate the spatial view, left, right, behind, in realtion to the Agent in the 3D environment.  So there is no real spatial awareness connection. 

Note I had to seperate the Chat Intercations from the Vision requests. So there is some discontinuity between the vision and the conversational contextes. 






## Further Work and Development ##

I have to understand and introduce a Command Context within the OpenAI Chat Sessions. So that 


## Acknowledgements ##

- [Open AI GPT API Service](https://platform.openai.com/docs/api-reference/introduction)
- [OKGoDolt A C# Wrapper for OPEN AI Calls](https://github.com/OkGoDoIt/OpenAI-API-dotnet)
- [FrostWeep Games: Google Text to Speech Wrapper Asset]( https://assetstore.unity.com/packages/add-ons/machinelearning/text-to-speech-using-google-cloud-pro-115170#description)
-


