# Dynamic Endpoints

This repo serves a proof of concept for generating dynamic endpoints and binding them during runtime. 

## Usage
1. Run the project
2. Open your browser and enter `http://localhost:5087/EndpointGenerator?name=nameOfYourChoice`
3. Press enter
4. Wait for the message `Created nameOfYourChoice`
5. Open a new tab and enter `http://localhost:5087/nameOfYourChoice` and be amazed!

## What is going to happen
When sending a request to the EndpointGenerator, it is going to replace all `{PLACEHOLDER}`s in the template file `Templates/default.txt` with the given name. 
It will then compile the altered Template and create a .dll. This dll is then added as a controller, which can subsequently be called. 
Cool, isn't it?