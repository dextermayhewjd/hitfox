# hitfox

## Table of Contents 
- [Group Details](#group-details)
- [Description of the project](#description-of-the-project)
- [How to develop using C# and unity](#how-to-develop-using-c-and-unity)
- [What does the folders contains](#what-does-the-folders-contains)
- [Development log book](#development-log-book)

## Deploying the game
1-unzip zip folder submitted
2-navigate to dev folder 
3- run the hitfox.exe application located in the folder
4-(Optiona) Change user tag 
5- create or find a room 
5-Play the game

## Group Details
Team member: Dexter Ding, Hailey Liney, Tuesday Hands, Azmil Roslan, Elan Virtucio, Jakub Navratil, Abdulrahman Abdulaal

## Description of the project
This is a team project that is developed by seven University of Bristol CS MEng student  

Here is the Link for the game idea : [hitfox idea](https://docs.google.com/document/d/1GQw3GEfUrCOAK0CKi3Sczs0TBW-TibXbnsBiwvKPHNw/edit?usp=sharing) 



## How to develop using C# and unity 
1.You should first download the unity 2022.1.23f1(https://unity.com/releases/editor/whats-new/2022.1.23)  
2. Since you are using C# obviously you need to download the [.Net](https://dotnet.microsoft.com/en-us/)  
3. For the VS entension please ensure you got the `c#` `Unity Code Snippets` `VS Live Share`  
4. Also please refer to the [documentation of unity](https://docs.unity3d.com/ScriptReference/index.html)    

## How to get the model of the Royal Fort Garden 
1. download the [blender](https://www.blender.org/thanks/) and install the [RenderDoc](https://renderdoc.org/builds)
2. the offical guildance of how to use this is [here](https://github.com/eliemichel/MapsModelsImporter) 


## What does the folders contains
1. All the C# scripts that are stored in the `Asset/Script`  
2. The script that contains the movement logic of the player is in the `Assets/Script/Player.cs`  
3. The basic scene that contains the moveable cube in the `Asset/Scenes/basic.unity`


## Development log book 
Used by Everyone to keep track of the development process

2023-1-25   


    a working cube that is able to move using WASD key 
    the the moving area is restricted       
    
    the start() define the starting point of the cube 
    
    the update is called per frame
    
    the movement of the object is updated using transform.Translate

    the restriced area by updating the position using if statement and transform.position  
    by Dexter Ding
