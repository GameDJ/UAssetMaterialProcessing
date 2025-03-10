using System;
using System.Collections.Generic;

public class MagikVfxTesting : MaterialTargetGenerator {
    public static List<FileTarget> GenerateTargets() {
        List<FileTarget> fileTargets = new List<FileTarget>();
        Func<string, bool> colorTargeter = name => name.ToLower().Contains("color") && !name.ToLower().Contains("enemy");
        Func<float[], float[]> preserveSL_Mapper = rgb => ColorOps.NormalizeThenFunctionThenUnnormalize(
            rgb, normalizedRGB => ColorOps.ModifyHSL(normalizedRGB,
                ModifyH: _hue => 300));
        Func<float[], float[]> overwrite_Mapper = _ => [1, 0, 0];
        Func<float[], float[]> preserveIntensity_mapper = rgb => ColorOps.NormalizeThenFunctionThenUnnormalize(
            rgb, normalizedRGB => ColorOps.ModifyHSL(normalizedRGB,
                ModifyHSL: _ => new ColorHelper.HSL(300, 100, 50)));

        string magikVfxMatPrefix = "Marvel\\Content\\Marvel\\VFX\\Materials\\Characters\\1029\\Materials\\";
        List<string> magikVfxFileNames = FileOps.GetFileNamesInDirectory("uassets\\" + magikVfxMatPrefix);
        magikVfxFileNames = magikVfxFileNames.FindAll(name => !name.ToLower().Contains("1029500"));
        foreach (string magikVfxFileName in magikVfxFileNames) {
            FileTarget magikVfxTarget = new FileTarget(magikVfxMatPrefix, magikVfxFileName);
            magikVfxTarget.AddVectorTarget(colorTargeter, preserveIntensity_mapper);
            fileTargets.Add(magikVfxTarget);
        }
        return fileTargets;
    }
}