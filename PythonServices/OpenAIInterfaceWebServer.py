# ========================================================================
# JSON processing - Need to use Postman App to send Json and review 
#  See https://www.youtube.com/watch?v=kvux1SiRIJQ
# ============================================================================== 
print()
print(" [INFO]: Imports .... ")

from flask import Flask, render_template, request,jsonify,json
from flask_cors import CORS
import random, math, copy
import json
#
from openai import OpenAI
import base64
import requests
import os

# ==============================================================================
#
app = Flask(__name__)
# ====================================================================
# Persistent Variables
ResponseCount = 0
LastCheckCount = 0
LastOpenAIViewResponse = ""
#
# OpenAI API Key
api_key = os.environ.get('OPEN_AI_KEY')
#
# =========================================================
# Function to encode the image
def encode_image(image_path):
  with open(image_path, "rb") as image_file:
    return base64.b64encode(image_file.read()).decode('utf-8')
# ====================================================================
@app.route("/")
def hello():
    print("[INFO]: Test Get Request ")
    return "Hello: This the Python Web OpenAI Server For Unity !"
# ========================================================================
# JSON processing - Need to use Postman App to send Json and review 
@app.route("/requestview",methods=['POST'])
def process_view_request():
    global ResponseCount,LastOpenAIViewResponse
    # ===============
    LastOpenAIViewResponse = ""
    print(" [INFO]: Processing View Request .... ")
    # Getting Access to the Subject Image
    image_path = "D:/AIEnvironmentSnapshots/output.png"
    # Getting the base64 string
    base64_image = encode_image(image_path)
    # =============================================================================
    # Setting up the Request Http Message
    # 
    print(" [INFO]: Creating OpenAI Web Request .... ")
    #
    headers = {
    "Content-Type": "application/json",
    "Authorization": f"Bearer {api_key}"
    }
    # ===========================
    payload = {
    "model": "gpt-4o",
    "messages": [
        {
        "role": "user",
        "content": [
            {
            "type": "text",
            "text": "Excluding the open field, please just list the main features in the foreground of this picture?"
            },
            {
            "type": "image_url",
            "image_url": {
                "url": f"data:image/jpeg;base64,{base64_image}"
            }
            }
        ]
        }
    ],
    "max_tokens": 300
    }
    # ============================
    #  Now Make the OpenAI Web Request
    response = requests.post("https://api.openai.com/v1/chat/completions", headers=headers, json=payload)
    #
    # ==================
    # Process the Response
    JsonResponse = response.json()
    LastOpenAIViewResponse = JsonResponse['choices'][0]['message']['content']
    print(" [INFO]: Received Following View Description .... ") 
    print(LastOpenAIViewResponse)
    print()
    ResponseCount = ResponseCount+1
    
    # ===================
    # Respond to Unity Web Request    
    return jsonify({'result':'Success!'})
# ==============================================================================
@app.route("/QueryPictureRequest",methods=['POST'])
def Process_QuerySurvey():
    global ResponseCount,LastCheckCount,LastOpenAIViewResponse
    print(" [INFO]: View Response Request ")
    if(LastCheckCount == ResponseCount):
        return jsonify({'result':'None','description':'None'})
    
    # Otherwise we have had a response from OpenAI
    print(" [INFO]: Returning View Description ")
    LastCheckCount = ResponseCount 
    return jsonify({'result':'Success','description':LastOpenAIViewResponse})
# ==============================================================================

# Main method to Start the Web Server - avoids setting up Environment variables
if __name__ == "__main__":
    print()
    print("[INFO] Running the Unity Support  Web Server ")
    app.run(debug=True,port = 7070)			# Note set to Debug to avoid having to restart the python Web App Server upon changs		