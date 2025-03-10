using System;
using System.Collections.Generic;

public class StormGreen : MaterialTargetGenerator {
    public static List<FileTarget> GenerateTargets() {
        List<FileTarget> fileTargets = new List<FileTarget>();

        Func<string, bool> colorTargeter = name => name.ToLower().Contains("color");// && !name.ToLower().Contains("enemy");
        //Func<float[], float[]> preserveSL_Mapper = rgb => ColorOps.NormalizeThenFunctionThenUnnormalize(
        //    rgb, normalizedRGB => ColorOps.ModifyHSL(normalizedRGB,
        //        ModifyH: _hue => 300));
        Func<float[], float[]> preserveIntensity_Mapper = rgb => ColorOps.NormalizeThenFunctionThenUnnormalize(
            rgb, normalizedRGB => ColorOps.ModifyHSL(normalizedRGB,
                ModifyHSL: _ => new ColorHelper.HSL(90, 100, 50)));
        Func<float[], float[]> overwrite_Mapper = _ => [1, 2, 0];

        string matPrefix = "Marvel\\Content\\Marvel\\VFX\\Materials\\Characters\\1015\\Materials\\";
        //string matPrefix = "Marvel\\Content\\Marvel\\VFX\\Materials\\Common\\Lightning\\";

        List<string> fileNames = FileOps.GetFileNamesInDirectory("uassets\\" + matPrefix);
        foreach (string fileName in fileNames) {
            FileTarget matTarget = new FileTarget(matPrefix, fileName);
            matTarget.AddVectorTarget(name => name == "Contact_Glow", _ => [25, 50, 0]);
            matTarget.AddVectorTarget(colorTargeter, preserveIntensity_Mapper);
            fileTargets.Add(matTarget);
        }
        return fileTargets;
    }
}