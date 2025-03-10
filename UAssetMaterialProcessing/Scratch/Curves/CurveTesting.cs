using System;
using System.Collections.Generic;

public class CurveTesting
{
	public CurveTesting()
	{
	}

    static void CurveTestFunc() {
        //UAsset myAsset = new UAsset("uassets\\Marvel\\Content\\Marvel\\VFX\\Materials\\Characters\\1015\\Curves\\CurveAtlas_1015_2_001.uasset", engineVersion, mappings);
        UAsset myAsset = new UAsset("uassets\\Marvel\\Content\\Marvel\\VFX\\Materials\\Characters\\1015\\Curves\\Curve_1015_6_606.uasset", engineVersion, mappings);
        NormalExport myExport = (NormalExport)myAsset.Exports[0];
        List<PropertyData> curveLinearColor = myExport.Data;
        RichCurve richCurve = new RichCurve();

        for (int i = 0; i < 4; i++) {
            List<PropertyData> rgbaKeyLists = ((StructPropertyData)curveLinearColor[i]).Value;
            PropertyData[] curChannelKeys = ((ArrayPropertyData)rgbaKeyLists[0]).Value;

            for (int j = 0; j < curChannelKeys.Length; j++) {
                FRichCurveKey channelKey = (FRichCurveKey)(((StructPropertyData)curChannelKeys[j]).Value[0]).RawValue;
                richCurve.AddKey(i, channelKey);
            }
        }
        richCurve.PrintKeys();

        Console.WriteLine();
        Console.WriteLine(String.Join(", ", richCurve.GetColorAtTime(0.7181146f)));
        Console.WriteLine(String.Join(", ", richCurve.GetColorAtTime(0.19f)));
    }

    static void WriteStormCurves() {
        RichCurve richCurve1 = new RichCurve();
        richCurve1.AddRgbaKey(0, [0, 1, 0, 1]);
        richCurve1.PrintKeys();

        //string curveDirectory = "Marvel\\Content\\Marvel\\VFX\\Materials\\Characters\\1015\\Curves\\";
        string curveDirectory = "Marvel\\Content\\Marvel\\VFX\\Materials\\Characters\\1039\\Curves\\";
        List<string> fileNames = FileOps.GetFileNamesInDirectory("uassets\\" + curveDirectory);
        fileNames.RemoveAll(fileName => fileName.Contains("CA") || fileName.Contains("Atlas"));



        foreach (string fileName in fileNames) {
            Console.WriteLine(fileName);
            UAsset myAsset = new UAsset("uassets\\" + curveDirectory + fileName + ".uasset", engineVersion, mappings);
            NormalExport myExport = (NormalExport)myAsset.Exports[0];
            List<PropertyData> curveLinearColor = myExport.Data;
            RichCurve richCurve = new RichCurve();

            for (int i = 0; i < 4; i++) {
                List<PropertyData> rgbaKeyLists = ((StructPropertyData)curveLinearColor[i]).Value;
                if (rgbaKeyLists.Count == 0) {
                    // sometimes the alpha is empty and just uses a default value?
                    continue;
                }
                PropertyData[] curChannelKeys = ((ArrayPropertyData)rgbaKeyLists[0]).Value;

                for (int j = 0; j < curChannelKeys.Length; j++) {
                    //FRichCurveKey channelKey = (FRichCurveKey)(((StructPropertyData)curChannelKeys[j]).Value[0]).RawValue;
                    RichCurveKeyPropertyData curveKeyPropData = (RichCurveKeyPropertyData)(((StructPropertyData)curChannelKeys[j]).Value[0]);
                    //richCurve.AddKey(i, channelKey);
                    FRichCurveKey newKey = new FRichCurveKey();
                    // ensure times dont overlap if there are multiple keys
                    newKey.Time = (float)j / (float)curChannelKeys.Length;
                    // green and alpha set to 1; others stay at 0
                    if (i == 1 || i == 3) {
                        newKey.Value = 1f;
                    }
                    curveKeyPropData.SetObject(newKey);
                }
            }
            //string editedDir = "edited\\StormCurvesGreen-v01_P\\";
            string editedDir = "edited\\ThorCurvesRed-v01_P\\";
            System.IO.Directory.CreateDirectory(editedDir + curveDirectory);
            myAsset.Write(editedDir + curveDirectory + fileName + ".uasset");
        }
    }


}
