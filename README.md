# UAssetMaterialProcessing

## Setting up the project
Sorry I don't know a lot about it. Please refer to the UAssetAPI instructions: https://github.com/atenfyr/UAssetAPI  
You may need to use Nuget to download the packages ColorHelper, ZstdSharp.Port v0.8.1, and possibly ZstdNet v1.4.5.

## How to use
Once things are set up, you'll need to do a few things in Program.cs: 
 - Set the path to the mappings file
 - Copy any assets to be edited into bin/Debug/net8.0/uassets/ using the whole directory structure starting from Marvel/Content/Marvel
 - Generate a list of targets to modify
 	- Extend the MaterialTargetGenerator class and override GenerateTargets()
	- For tutorials on how this works, see ExampleTargetGenerator_Basic and ExampleTargetGenerator_Advanced
 	- I've also included several full functions I made for different batch processes, such as for Magik's Eldritch Armor
   
