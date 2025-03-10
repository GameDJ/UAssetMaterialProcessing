using System;
using System.Collections.Generic;

public class PsylockeButterflies : MaterialTargetGenerator {
    public static List<FileTarget> GenerateTargets() {
        List<FileTarget> fileTargets = new List<FileTarget>();

        Func<string, bool> colorTargeter = name => name.ToLower().Contains("color");// && !name.ToLower().Contains("enemy");
        //Func<float[], float[]> preserveSL_Mapper = rgb => ColorOps.NormalizeThenFunctionThenUnnormalize(
        //    rgb, normalizedRGB => ColorOps.ModifyHSL(normalizedRGB,
        //        ModifyH: _hue => 300));
        //Func<float[], float[]> preserveIntensity_Mapper = rgb => ColorOps.NormalizeThenFunctionThenUnnormalize(
        //    rgb, normalizedRGB => ColorOps.ModifyHSL(normalizedRGB,
        //        ModifyHSL: _ => new ColorHelper.HSL(90, 100, 50)));
        Func<float[], float[]> overwrite_Mapper = _ => [0, 0, 0];

        string matPrefix = "Marvel\\Content\\Marvel\\VFX\\Materials\\Characters\\1048\\Materials\\";
        //string matPrefix = "Marvel\\Content\\Marvel\\VFX\\Materials\\Common\\Lightning\\";

        List<string> fileNames = FileOps.GetFileNamesInDirectory("uassets\\" + matPrefix);
        fileNames = fileNames.FindAll(name => name.ToLower().Contains("butterfl"));
        foreach (string fileName in fileNames) {
            FileTarget matTarget = new FileTarget(matPrefix, fileName);
            matTarget.AddVectorTarget(colorTargeter, overwrite_Mapper);
            fileTargets.Add(matTarget);
        }
        return fileTargets;
    }
}