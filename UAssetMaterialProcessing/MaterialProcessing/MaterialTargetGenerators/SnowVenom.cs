using System;
using System.Collections.Generic;

public class SnowVenom : MaterialTargetGenerator {
    public static List<FileTarget> GenerateTargets() {
        List<FileTarget> fileTargets = new List<FileTarget>();

        Func<string, bool> colorTargeter = name => (name.ToLower().Contains("color") && !name.ToLower().Contains("softness")) || name == "MSR" || name == "MC_ShadeSmoothness_3";// && !name.ToLower().Contains("enemy");
        //Func<float[], float[]> preserveSL_Mapper = rgb => ColorOps.NormalizeThenFunctionThenUnnormalize(
        //    rgb, normalizedRGB => ColorOps.ModifyHSL(normalizedRGB,
        //        ModifyH: _hue => 300));
        Func<float[], float[]> preserveIntensity_greenMapper = rgb => ColorOps.NormalizeThenFunctionThenUnnormalize(
            rgb, normalizedRGB => ColorOps.ModifyHSL(normalizedRGB,
                ModifyHSL: _ => new ColorHelper.HSL(120, 100, 50)));
        Func<float[], float[]> preserveIntensity_purpleMapper = rgb => ColorOps.NormalizeThenFunctionThenUnnormalize(
            rgb, normalizedRGB => ColorOps.ModifyHSL(normalizedRGB,
                ModifyHSL: _ => new ColorHelper.HSL(260, 100, 50)));
        Func<float[], float[]> preserveIntensity_magentaMapper = rgb => ColorOps.NormalizeThenFunctionThenUnnormalize(
            rgb, normalizedRGB => ColorOps.ModifyHSL(normalizedRGB,
                ModifyHSL: _ => new ColorHelper.HSL(300, 100, 50)));
        Func<float[], float[]> green_overwriteMapper = _ => [0, 1, 0];
        Func<float[], float[]> magenta_overwriteMapper = _ => [1, 0, 1];

        //string matPrefix = "Marvel\\Content\\Marvel\\VFX\\Materials\\Characters\\1015\\Materials\\";
        string generalMatPrefix = "Marvel\\Content\\Marvel\\VFX\\Materials\\Characters\\1035\\Materials\\";
        string skinSideMatPrefix = "Marvel\\Content\\Marvel\\VFX\\Materials\\Characters\\1035\\1035300\\Materials\\";
        string skinMainMatPrefix = "Marvel\\Content\\Marvel\\VFX\\Materials\\Characters\\1035\\Materials\\1035300\\";
        string extraVfxMatPrefix = "Marvel\\Content\\Marvel\\VFX\\Materials\\Characters\\1035\\103561\\Materials\\";

        List<string> charMatPrefixes = new List<string>();
        charMatPrefixes.Add("Marvel\\Content\\Marvel\\Characters\\1035\\1035300\\Materials\\");
        charMatPrefixes.Add("Marvel\\Content\\Marvel\\Characters\\1035\\1035300\\Materials\\Lobby\\");
        //charMatPrefixes.Add("Marvel\\Content\\Marvel\\Characters\\1035\\1035300\\Weapons\\Ball\\Materials\\");
        //charMatPrefixes.Add("Marvel\\Content\\Marvel\\Characters\\1035\\1035300\\Weapons\\DevouringSymbiote\\Materials\\");
        //charMatPrefixes.Add("Marvel\\Content\\Marvel\\Characters\\1035\\1035300\\Weapons\\DevouringSymbiote_Hand\\Materials\\");
        //charMatPrefixes.Add("Marvel\\Content\\Marvel\\Characters\\1035\\1035300\\Weapons\\MVP\\SnowMan\\Materials\\");
        //charMatPrefixes.Add("Marvel\\Content\\Marvel\\Characters\\1035\\1035300\\Weapons\\MVP\\Teeth\\Materials\\");
        //charMatPrefixes.Add("Marvel\\Content\\Marvel\\Characters\\1035\\1035300\\Weapons\\SpikedTentacles\\Materials\\");

        //List<string> fileNames = FileOps.GetFileNamesInDirectory("uassets\\" + skinMainMatPrefix);
        //foreach (string fileName in fileNames) {
        //    FileTarget matTarget = new FileTarget(skinMainMatPrefix, fileName);
        //    matTarget.AddVectorTarget(colorTargeter, preserveIntensity_greenMapper);
        //    fileTargets.Add(matTarget);
        //}
        //List<string> sideMatNames = FileOps.GetFileNamesInDirectory("uassets\\" + skinSideMatPrefix);
        //foreach (string fileName in sideMatNames) {
        //    FileTarget matTarget = new FileTarget(skinSideMatPrefix, fileName);
        //    matTarget.AddVectorTarget(colorTargeter, preserveIntensity_magentaMapper);
        //    fileTargets.Add(matTarget);
        //}

        List<string> extraVfxMatNames = FileOps.GetFileNamesInDirectory("uassets\\" + extraVfxMatPrefix);
        foreach (string fileName in extraVfxMatNames) {
            FileTarget matTarget = new FileTarget(extraVfxMatPrefix, fileName);
            matTarget.AddVectorTarget(colorTargeter, preserveIntensity_purpleMapper);
            fileTargets.Add(matTarget);
        }

        //foreach (string charMatPrefix in charMatPrefixes) {
        //    List<string> charMatNames = FileOps.GetFileNamesInDirectory("uassets\\" + charMatPrefix);
        //    foreach (string fileName in charMatNames) {
        //        FileTarget matTarget = new FileTarget(charMatPrefix, fileName);
        //        matTarget.AddVectorTarget(colorTargeter, preserveIntensity_magentaMapper);
        //        fileTargets.Add(matTarget);
        //    }
        //}
        return fileTargets;
    }
}