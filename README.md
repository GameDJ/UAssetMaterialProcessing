# UAssetMaterialProcessing

## Setting up the project
Sorry I don't know a lot about it. Please refer to the UAssetAPI instructions: https://github.com/atenfyr/UAssetAPI
You may need to use Nuget to download the packages ColorHelper, ZstdSharp.Port v0.8.1, and possibly ZstdNet v1.4.5.

## How to use
Once things are set up, you'll need to do a few things: 
 - Set the path to the mappings file in Program.cs
 - Set the path to a folder with the assets you want to edit
	- I recommend copying them into a uassets/ folder and using the whole directory structure starting from Marvel/
 - Generate a list of targets to modify
	- See the functions "SampleTargetGenerator_Basic" and "SampleTargetGenerator_Advanced" for tutorials on how to do this
	- 