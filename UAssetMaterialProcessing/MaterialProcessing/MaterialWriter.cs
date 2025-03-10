using System;
using System.Collections.Generic;
using UAssetAPI.UnrealTypes;
using UAssetAPI;
using UAssetAPI.Unversioned;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using System.IO;

public class MaterialWriter {
    EngineVersion engineVersion;
    Usmap mappings;
    string fileSuffix = ".uasset";

    public MaterialWriter(EngineVersion engineVersion, Usmap mappings) {
        this.engineVersion = engineVersion;
        this.mappings = mappings;
    }

    public void ModifyAndWrite(List<FileTarget> fileTargets, string outputDirectoryPrefix = "") {
        foreach (FileTarget target in fileTargets) {
            UAsset? myAsset = new UAsset($"uassets\\{target.localPathPrefix}{target.name}{this.fileSuffix}", this.engineVersion, this.mappings);
            myAsset = ModifyTargets(
                myAsset,
                target.scalarTargets,
                target.vectorTargets,
                printOutput: true,
                printFilename: target.name
            );
            if (myAsset != null) {
                string fullOutputPath = $"edited\\{(outputDirectoryPrefix == "" ? "" : outputDirectoryPrefix + "\\")}{target.localPathPrefix}{target.name}{this.fileSuffix}";
                System.IO.Directory.CreateDirectory(fullOutputPath.Substring(0, fullOutputPath.LastIndexOf("\\")));
                myAsset.Write(fullOutputPath);
            }

        }
    }

    /// <summary>
    /// Takes a UAsset object loaded from a material file, locates scalars/vectors whose names match the provided conditional functions,
    ///  and performs associated operations on their values.
    /// </summary>
    /// <param name="myAsset"></param>
    /// <param name="scalarTargets"></param>
    /// <param name="vectorTargets"></param>
    /// <param name="printOutput">Whether to print changes to the console (recommended)</param>
    /// <param name="printFilename">Filename to print to the console next to the changes</param>
    /// <returns>The edited UAsset object</returns>
    private static UAsset? ModifyTargets(
        UAsset myAsset,
        List<Tuple<Func<string, bool>, Func<float, float>>>? scalarTargets,
        List<Tuple<Func<string, bool>, Func<float[], float[]>>>? vectorTargets,
        bool printOutput = false,
        string printFilename = ""
    ) {
        if (printOutput) Console.WriteLine(printFilename);

        // Material Instance Constant
        NormalExport myExport = (NormalExport)myAsset.Exports[0];

        // Modify scalars
        if (scalarTargets != null) {
            // Scalar Parameter Values
            if (myExport["ScalarParameterValues"] == null) return null;
            PropertyData[] scalars = (PropertyData[])myExport["ScalarParameterValues"].RawValue;
            for (int i = 0; i < scalars.Length; i++) {
                // scalar list item
                List<PropertyData> scalar = (List<PropertyData>)scalars[i].RawValue;
                // Check the parameter name
                StructPropertyData parameterInfo = (StructPropertyData)scalar[0];
                string? nameProperty = ((PropertyData)parameterInfo["Name"]).ToString();
                if (nameProperty == null) {
                    Console.Error.WriteLine($"Error getting name property for scalar {i} of {printFilename}. parameterInfo: {parameterInfo}");
                } else {
                    //Tuple<Func<string, bool>, Func<float, float>> scalarTargetFuncs = scalarTargets.Find(targetFuncs => targetFunc[0](nameProperty));
                    Tuple<Func<string, bool>, Func<float, float>>? scalarTargetFuncs = scalarTargets.Find(targetFuncs => targetFuncs.Item1(nameProperty));
                    if (scalarTargetFuncs != null) {
                        //if (scalarTargetFuncs != default(Tuple<Func<string, bool>, Func<float, float>>)) {
                        Func<float, float> scalarModifyFunc = scalarTargetFuncs.Item2;

                        //// Get the parameter value
                        FloatPropertyData parameterValue = (FloatPropertyData)scalar[1];

                        if (scalarModifyFunc == null) {
                            Console.Error.WriteLine($"Error getting modify func for scalar {i} of {printFilename}. nameProperty: {nameProperty}");
                        } else if (parameterValue == null) {
                            if (printOutput) {
                                Console.WriteLine($"\t{nameProperty}");
                                Console.WriteLine($"\t\tUnable to read scalar value");
                            }
                        } else {
                            if (printOutput) {
                                Console.WriteLine($"\t{nameProperty}");
                                Console.WriteLine($"\t\t{parameterValue.Value}");
                            }
                            float modifiedValue = scalarModifyFunc(parameterValue.Value);
                            parameterValue.Value = modifiedValue;
                            if (printOutput) {
                                Console.WriteLine($"\t\t{parameterValue.Value}");
                            }
                        }
                    }
                }
            }
        }

        // Modify vectors
        if (vectorTargets != null) {
            // Vector Parameter Values
            if (myExport["VectorParameterValues"] == null) return null;
            PropertyData[] vectors = (PropertyData[])myExport["VectorParameterValues"].RawValue;
            for (int i = 0; i < vectors.Length; i++) {
                // vector list item
                List<PropertyData> vector = (List<PropertyData>)vectors[i].RawValue;
                // Check the parameter name
                StructPropertyData parameterInfo = (StructPropertyData)vector[0];
                string? nameProperty = ((PropertyData)parameterInfo["Name"]).ToString();
                if (nameProperty == null) {
                    Console.Error.WriteLine($"Error getting name property for vector {i} of {printFilename}. parameterInfo: {parameterInfo}");
                } else {
                    Tuple<Func<string, bool>, Func<float[], float[]>>? vectorTargetFuncs = vectorTargets.Find(targetFuncs => targetFuncs.Item1(nameProperty));

                    if (vectorTargetFuncs != null) {
                        //if (scalarTargetFuncs != default(Tuple<Func<string, bool>, Func<float, float>>)) {
                        Func<float[], float[]> vectorModifyFunc = vectorTargetFuncs.Item2;

                        // Get the parameter value struct
                        StructPropertyData parameterValue = (StructPropertyData)vector[1];
                        // Get the container for the actual value out of the struct
                        LinearColorPropertyData rgba = (LinearColorPropertyData)parameterValue["ParameterValue"];

                        if (vectorModifyFunc == null) {
                            Console.Error.WriteLine($"Error getting modify func for vector {i} of {printFilename}. nameProperty: {nameProperty}");
                        } else if (rgba == null) {
                            if (printOutput) {
                                Console.WriteLine($"\t{nameProperty}");
                                Console.WriteLine($"\t\tUnable to read vector");
                            }
                        } else {
                            if (printOutput) {
                                Console.WriteLine($"\t{nameProperty}");
                                Console.WriteLine("\t\t{0,8} {1, 8} {2, 8} {3, 8}", rgba.Value.R * 255, rgba.Value.G * 255, rgba.Value.B * 255, rgba.Value.A);
                            }
                            float[] modifiedValue = vectorModifyFunc([(float)rgba.Value.R, (float)rgba.Value.G, (float)rgba.Value.B]);
                            rgba.SetObject(new FLinearColor(modifiedValue[0], modifiedValue[1], modifiedValue[2], rgba.Value.A));
                            if (printOutput) {
                                Console.WriteLine("\t\t{0,8} {1, 8} {2, 8} {3, 8}", rgba.Value.R * 255, rgba.Value.G * 255, rgba.Value.B * 255, rgba.Value.A);
                            }
                        }
                    }
                }
            }
        }

        if (printOutput) {
            Console.WriteLine();
        }
        return myAsset;
    }
}