using ColorHelper;
using System;
using System.Collections.Generic;
using System.IO;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;
using UAssetAPI;
using UAssetAPI.Unversioned;

public static class FileOps {
    /// <summary>
    /// Gets the name (no path, no extension) of all files in the given directory
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static List<string> GetFileNamesInDirectory(string directory) {
        string[] files = Directory.GetFiles(directory);
        List<string> fileNames = new List<string>();
        for (int i = 0; i < files.Length; i++) {
            if (!files[i].Contains(".uasset")) {
                string[] splitFilePath = files[i].Split(@"\");
                string fileNameWithSuffix = splitFilePath[splitFilePath.Length - 1];
                string fileName = fileNameWithSuffix.Substring(0, fileNameWithSuffix.LastIndexOf("."));
                fileNames.Add(fileName);
            }
        }
        return fileNames;
    }

    public static void PrintAllMaterialColorsInDirectory(string directory, EngineVersion engineVersion, Usmap mappings) {
        List<string> fileNames = FileOps.GetFileNamesInDirectory(directory);
        foreach (string fileName in fileNames) {
            FileOps.PrintMaterialColors(directory, fileName, engineVersion, mappings);
        }
    }

    /// <summary>
    /// Print info about all color vectors in the material
    /// </summary>
    /// <param name="directory"></param>
    public static void PrintMaterialColors(string pathPrefix, string fileName, EngineVersion engineVersion, Usmap mappings) {
            UAsset myAsset = new UAsset($"{pathPrefix}\\{fileName}.uasset", engineVersion, mappings);

            Console.WriteLine(fileName);

            // Material Instance Constant
            NormalExport myExport = (NormalExport)myAsset.Exports[0];

            // Vector Parameter Values
            if (myExport["VectorParameterValues"] == null) return;

            PropertyData[] vectors = (PropertyData[])myExport["VectorParameterValues"].RawValue;
            for (int i = 0; i < vectors.Length; i++) {
                // vector list item
                List<PropertyData> vector = (List<PropertyData>)vectors[i].RawValue;
                // Check the parameter name
                StructPropertyData parameterInfo = (StructPropertyData)vector[0];
                string? nameProperty = ((PropertyData)parameterInfo["Name"]).ToString();
                if (nameProperty == null) {
                    Console.Error.WriteLine($"Error getting name property for vector {i} of {fileName}. parameterInfo: {parameterInfo}");
                } else if (nameProperty.ToLower().Contains("color")) {
                    // Get the parameter value struct
                    StructPropertyData parameterValue = (StructPropertyData)vector[1];
                    // Get the container for the actual value out of the struct
                    LinearColorPropertyData rgba = (LinearColorPropertyData)parameterValue["ParameterValue"];

                    Console.WriteLine($"\t{nameProperty}");
                    if (rgba == null) {
                        Console.WriteLine($"\t\tUnable to read vector");
                    } else {
                        Console.WriteLine("\t\tRGB: {0,10} {1, 10} {2, 10} {3, 10}", rgba.Value.R * 255, rgba.Value.G * 255, rgba.Value.B * 255, rgba.Value.A);
                        HSL hsl = ColorOps.RGBtoHSL([rgba.Value.R, rgba.Value.G, rgba.Value.B]);
                        Console.WriteLine("\t\tHSL: {0,4} {1, 4} {2, 4}", hsl.H, hsl.S, hsl.L);
                        Console.WriteLine($"\t\t{ColorOps.GetColorName(hsl)}");
                    }
                }
            }
    }
}