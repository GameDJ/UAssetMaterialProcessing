using System;
using System.Collections.Generic;

public class JeffWater : MaterialTargetGenerator {
    public static List<FileTarget> GenerateTargets() {
        List<FileTarget> fileTargets = new List<FileTarget>();
        string jeffVfxLocalPrefix = @"Marvel\Content\Marvel\VFX\Materials\Characters\1047\Materials\";
        List<string> jeffVfxFileNames = FileOps.GetFileNamesInDirectory("uassets\\" + jeffVfxLocalPrefix);
        foreach (string jeffVfxFileName in jeffVfxFileNames) {
            FileTarget jeffVfxTarget = new FileTarget(jeffVfxLocalPrefix, jeffVfxFileName);

            // Targeting function
            Func<string, bool> jeffVfxTargetFunc = name => name.ToLower().Contains("color") && !name.ToLower().Contains("enemy");

            // Modifier function
            Func<float[], float[]> yellowMapper = rgb => ColorOps.NormalizeThenFunctionThenUnnormalize(
            rgb, normalizedRGB => ColorOps.ModifyHSL(normalizedRGB,
                ModifyH: _hue => 55));

            jeffVfxTarget.AddVectorTarget(jeffVfxTargetFunc, yellowMapper);

            fileTargets.Add(jeffVfxTarget);
        }

        return fileTargets;
    }
}