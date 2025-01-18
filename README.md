# UAssetMaterialProcessing

## Setting up the project
Sorry I don't know a lot about it. Please refer to the UAssetAPI instructions: https://github.com/atenfyr/UAssetAPI  
You may need to use Nuget to download the packages ColorHelper, ZstdSharp.Port v0.8.1, and possibly ZstdNet v1.4.5.

## How to use
Once things are set up, you'll need to do a few things in Program.cs: 
 - Set the path to the mappings file
 - Set the path to a folder with the assets you want to edit
	- I recommend copying the base assets into a uassets/ folder and within it using the whole directory structure starting from Marvel/
 		- The output files will be generated with the same directory structure 
 - Generate a list of targets to modify
	- There are two functions in Program.cs, starting with "SampleTargetGenerator" for a basic and advanced tutorial on how to do this
 	- I also included the full function I use for generating my material targets for Magik's Eldritch Armor
   
