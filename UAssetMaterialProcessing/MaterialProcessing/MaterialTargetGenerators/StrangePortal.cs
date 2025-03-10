using System;
using System.Collections.Generic;

public class StrangePortal : MaterialTargetGenerator {
    public static List<FileTarget> GenerateTargets() {
        List<FileTarget> fileTargets = new List<FileTarget>();
        string strangeVfxLocalPathPrefix = "Marvel\\Content\\Marvel\\VFX\\Materials\\Characters\\1018\\Materials\\";
        Dictionary<string, int> fileNameHueMapperDict = new Dictionary<string, int>();
        //List<string> strangeVfxFileNames = GetFileNamesInDirectory(inputPathPrefix + strangeVfxLocalPathPrefix);
        //for (int i=0; i < strangeVfxFileNames.Count; i++) {
        //string filename = strangeVfxFileNames[i];
        foreach (string filename in new string[] { "MI_Fire_13_311", "MI_Fire_13_312", "MI_Fire_13_313" }) {
            FileTarget fileTarget = new FileTarget(strangeVfxLocalPathPrefix, filename);
            Func<string, bool> sampleVectorTargetFunc = name => (name.ToLower().Contains("color") || name == "Add_Bloom") && !name.ToLower().Contains("enemy");

            fileTarget.AddVectorTarget(sampleVectorTargetFunc, _ => [0, 1, 0.8f]);
            fileTargets.Add(fileTarget);
        }

        return fileTargets;
    }
}