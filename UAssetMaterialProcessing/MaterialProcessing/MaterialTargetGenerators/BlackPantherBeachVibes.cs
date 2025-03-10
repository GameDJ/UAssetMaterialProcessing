using ColorHelper;
using System;
using System.Collections.Generic;

public class BlackPantherBeachVibes : MaterialTargetGenerator {
    public static List<FileTarget> GenerateTargets() {
        List<FileTarget> fileTargets = new List<FileTarget>();

        Func<string, bool> colorTargeter = name => name.ToLower().Contains("color") && !name.ToLower().Contains("enemy") && !name.ToLower().Contains("softness");

        Func<float[], float[]> beachPinkMapper = rgb => ColorOps.NormalizeThenFunctionThenUnnormalize(
            rgb, normalizedRGB => ColorOps.ModifyHSL(normalizedRGB,
                ModifyHSL: origHSL => {
                    if (origHSL.S == 0) return origHSL;
                    int newHue = ColorOps.MapHueToGradient(250, 300, true, 347, 9, true, origHSL.H);
                    //// this is just using maphuetogradient to attempt to lerp sat based on the new hue
                    //byte newSat = (byte)ColorOps.MapHueToGradient(
                    //            316, 340, true,
                    //            45, 18, false,
                    //            newHue);
                    return new HSL(newHue, 30, 53);
                }
            )
        );

        string matPrefix = "Marvel\\Content\\Marvel\\VFX\\Materials\\Characters\\1026\\Materials\\";

        List<string> fileNames = FileOps.GetFileNamesInDirectory("uassets\\" + matPrefix);
        foreach (string fileName in fileNames) {
            FileTarget matTarget = new FileTarget(matPrefix, fileName);
            matTarget.AddVectorTarget(colorTargeter, beachPinkMapper);
            fileTargets.Add(matTarget);
        }
        return fileTargets;
    }
}