using System;
using System.Collections.Generic;

public class MagikEldritchRed : MaterialTargetGenerator {
    public static List<FileTarget> GenerateTargets() {
        List<FileTarget> fileTargets = new List<FileTarget>();

        string vfxMatPrefix = "Marvel\\Content\\Marvel\\VFX\\Materials\\Characters\\1029\\Materials\\";
        string charMatPrefix = "Marvel\\Content\\Marvel\\Characters\\1029\\1029500\\Materials\\";
        string filePrefix = "MI_1029500_";

        Func<string, bool> colorTargeter = name => name.ToLower().Contains("color") && !name.ToLower().Contains("enemy") && !name.ToLower().Contains("offset") && !name.ToLower().Contains("softness");

        Func<float[], float[]> blueCyanToRedGoldMapper = rgb => ColorOps.NormalizeThenFunctionThenUnnormalize(
            rgb,
            normalizedRGB => ColorOps.ModifyHSL(normalizedRGB,
                ModifyH: hue => ColorOps.MapHueToGradient(
                    210, 180, false,
                    0, 55, true,
                    hue
                ),
                ModifyS: _sat => 100,
                ModifyL: lightness => (byte)Math.Max(50, (100 * ColorOps.ShiftValueInRangeByPivot((float)lightness / 100f, 0.25f)))
            )
        );

        Func<float[], float[]> redMapper = rgb => ColorOps.NormalizeThenFunctionThenUnnormalize(
            rgb,
            normalizedRGB => ColorOps.ModifyHSL(normalizedRGB,
                ModifyH: _hue => 0
            )
        );

        // Add vfx "flow" materials
        int flowMax = 62;
        //int flowMax = 1;
        List<int> flowSkip = new List<int>
        {
            8, 23, 24, 26, 29, 58, 59
        };
        for (int i = 1; i <= flowMax; i++) {
        //for (int i = 14; i <= 14; i++) {
            if (flowSkip.Contains(i)) continue;

            string fileNameEnding = "Flow_26_0";
            if (i < 10) fileNameEnding += "0";
            fileNameEnding += i.ToString();

            FileTarget fileTarget = new FileTarget(vfxMatPrefix + filePrefix, fileNameEnding);
            Func<string, bool> generalVectorTargetFunc = colorTargeter;
            Func<float[], float[]> generalVectorModifyFunc;

            if (i == 7 || i == 27 || i == 43) {
                // shoulder outer x (human lobby, human ingame, and darkchylde)
                if (i == 7) {
                    // Add this target first to override BaseColor
                    fileTarget.AddVectorTarget(name => name == "BaseColor", _ => [4, 0, 0]);
                }
                generalVectorModifyFunc = redMapper;
            } else {
                generalVectorModifyFunc = blueCyanToRedGoldMapper;
            }
            fileTarget.AddVectorTarget(generalVectorTargetFunc, generalVectorModifyFunc);
            fileTargets.Add(fileTarget);
        }
        // Add miscellaneous fire materials (not sure exactly what they're for)
        foreach (string endingNum in new string[] { "319", "320", "323", "324", "341", "342" }) {
            FileTarget fileTarget = new FileTarget(vfxMatPrefix + "MI_", "Fire_13_" + endingNum);
            fileTarget.AddVectorTarget(colorTargeter, blueCyanToRedGoldMapper);
            fileTargets.Add(fileTarget);
        }
        // Add character materials to fileItems
        foreach (string innerInnerPath in new string[] { "10290\\", "10290\\Lobby\\" }) {
            // Crown, chest gem, hilt, blade
            foreach (string fileName in new string[] { "Equip_04", "Equip_05", "Weapon_01", "Weapon_02" }) {
                FileTarget fileTarget = new FileTarget(charMatPrefix + innerInnerPath + filePrefix, "10290_" + fileName);
                if (fileName == "Equip_04" || fileName == "Equip_05") {
                    // Overrides for crown and chest gem
                    fileTarget.AddScalarTarget(name => name == "cubeMapStrength", _ => 5);
                    fileTarget.AddVectorTarget(name => name == "BasicLineColor", _ => [0.5f, 0, 0]);
                    fileTarget.AddVectorTarget(name => name == "RampColor1", _ => [1, 0, 0]);
                    fileTarget.AddVectorTarget(name => name == "InnerColor", _ => [75, 25, 0]);
                    fileTarget.AddVectorTarget(name => name == "RimLightColor", _ => [0.7f, 0.1f, 0]);
                    fileTarget.AddVectorTarget(name => name == "LineSpecColor", _ => [0.8f, 0, 0]);
                }
                fileTarget.AddVectorTarget(colorTargeter, redMapper);
                fileTargets.Add(fileTarget);
            }
        }
        // Lobby weapon effects
        foreach (string fileName in new string[] { "Weapon_02_FX_01", "Weapon_02_FX_02" }) {
            FileTarget fileTarget = new FileTarget(charMatPrefix + "10290\\Lobby\\" + filePrefix, "10290_" + fileName);
            fileTarget.AddVectorTarget(colorTargeter, redMapper);
            fileTargets.Add(fileTarget);
        }

        // Add Darkchylde materials
        foreach (string innerInnerPath in new string[] { "10291\\", "10291\\Lobby\\" }) {
            // Blade, Eyes, Hair
            foreach (string fileName in new string[] { "Weapon_02", "Eyes_01", "Hair_03" }) {
                FileTarget fileTarget = new FileTarget(charMatPrefix + innerInnerPath + filePrefix, "10291_" + fileName);
                if (fileName == "Weapon_02") {
                    // Overrides for sword blade
                    fileTarget.AddScalarTarget(name => name == "cubeMapStrength", _ => 4);
                    fileTarget.AddVectorTarget(name => name == "BasicLineColor", _ => [1, 0, 0]);
                    fileTarget.AddVectorTarget(name => name == "RampColor1", _ => [0, 0, 0]);
                    fileTarget.AddVectorTarget(name => name == "InnerColor", _ => [0, 0, 0]);
                    fileTarget.AddVectorTarget(name => name == "RimLightColor", _ => [1, 0, 0]);
                    fileTarget.AddVectorTarget(name => name == "LineSpecColor", _ => [0, 0, 0]);
                }
                fileTarget.AddVectorTarget(colorTargeter, redMapper);
                fileTargets.Add(fileTarget);
            }
        }

        // Add MVP materials
        string mvpFilePrefix = "MI_MVP_";
        foreach (string mvpNamePrefix in new string[] { "BackScreen_13_", "FadeOneMins_13_", "Fire_13_", "Flash_13_", "FreezeFire_13_", "Knife_13_", "Portal_13_", "Ring_13_", "Spark_13_", "Trail_13_", "Wind_13_" }) {
            if (mvpNamePrefix == "BackScreen_13_") {
                FileTarget fileTarget = new FileTarget(vfxMatPrefix, mvpFilePrefix + mvpNamePrefix + "301");
                fileTarget.AddVectorTarget(colorTargeter, redMapper);
                fileTargets.Add(fileTarget);
            } else if (mvpNamePrefix == "FadeOneMins_13_") {
                FileTarget fileTarget = new FileTarget(vfxMatPrefix, mvpFilePrefix + mvpNamePrefix + "301");
                fileTarget.AddVectorTarget(colorTargeter, redMapper);
                fileTargets.Add(fileTarget);
            } else if (mvpNamePrefix == "Fire_13_") {
                for (int i = 318; i <= 329; i++) {
                    if (i == 329) {
                        // skip from 328 to 339
                        i = 339;
                        continue;
                    }
                    FileTarget fileTarget = new FileTarget(vfxMatPrefix, mvpFilePrefix + mvpNamePrefix + i.ToString());
                    fileTarget.AddVectorTarget(colorTargeter, blueCyanToRedGoldMapper);
                    fileTargets.Add(fileTarget);
                }
            } else if (mvpNamePrefix == "Flash_13_") {
                foreach (string num in new string[] { "301", "302" }) {
                    FileTarget fileTarget = new FileTarget(vfxMatPrefix, mvpFilePrefix + mvpNamePrefix + num);
                    fileTarget.AddVectorTarget(colorTargeter, blueCyanToRedGoldMapper);
                    fileTargets.Add(fileTarget);
                }
            } else if (mvpNamePrefix == "FreezeFire_13_") {
                foreach (string num in new string[] { "301", "302", "303" }) {
                    FileTarget fileTarget = new FileTarget(vfxMatPrefix, mvpFilePrefix + mvpNamePrefix + num);
                    fileTarget.AddVectorTarget(colorTargeter, blueCyanToRedGoldMapper);
                    fileTargets.Add(fileTarget);
                }
            } else if (mvpNamePrefix == "Knife_13_") {
                FileTarget fileTarget = new FileTarget(vfxMatPrefix, mvpFilePrefix + mvpNamePrefix + "306");
                fileTarget.AddVectorTarget(colorTargeter, blueCyanToRedGoldMapper);
                fileTargets.Add(fileTarget);
            } else if (mvpNamePrefix == "Portal_13_") {
                foreach (string num in new string[] { "302", "303", "304" }) {
                    FileTarget fileTarget = new FileTarget(vfxMatPrefix, mvpFilePrefix + mvpNamePrefix + num);
                    fileTarget.AddVectorTarget(colorTargeter, blueCyanToRedGoldMapper);
                    fileTargets.Add(fileTarget);
                }
            } else if (mvpNamePrefix == "Ring_13_") {
                foreach (string num in new string[] { "301", "302" }) {
                    FileTarget fileTarget = new FileTarget(vfxMatPrefix, mvpFilePrefix + mvpNamePrefix + num);
                    fileTarget.AddVectorTarget(colorTargeter, blueCyanToRedGoldMapper);
                    fileTargets.Add(fileTarget);
                }
            } else if (mvpNamePrefix == "Trail_13_") {
                for (int i = 301; i <= 313; i++) {
                    FileTarget fileTarget = new FileTarget(vfxMatPrefix, mvpFilePrefix + mvpNamePrefix + i.ToString());
                    fileTarget.AddVectorTarget(colorTargeter, blueCyanToRedGoldMapper);
                    fileTargets.Add(fileTarget);
                }
            } else if (mvpNamePrefix == "Wind_13_") {
                foreach (string num in new string[] { "301", "302", "303", "306", "307" }) {
                    FileTarget fileTarget = new FileTarget(vfxMatPrefix, mvpFilePrefix + mvpNamePrefix + num);
                    fileTarget.AddVectorTarget(colorTargeter, blueCyanToRedGoldMapper);
                    fileTargets.Add(fileTarget);
                }
            }
        }

        // Magik Slash?

        Func<float[], float[]> trajectoryMapper = rgb => ColorOps.NormalizeThenFunctionThenUnnormalize(
            rgb, normalizedRGB => ColorOps.ModifyHSL(normalizedRGB,
                ModifyH: hue => ColorOps.MapHueToGradient(
                    30, 53, true,
                    0, 55, true,
                    hue
                ),
                ModifyS: _sat => 100,
                ModifyL: lightness => (byte)(100 * ColorOps.ShiftValueInRangeByPivot((float)lightness / 100f, 0.25f))
            )
        );

        foreach (string num in new string[] { "08", "09", "10", "11" }) {
            FileTarget fileTarget = new FileTarget(vfxMatPrefix + "MI_", "Trajectory_1_001_" + num);
            fileTarget.AddVectorTarget(colorTargeter, trajectoryMapper);
            fileTargets.Add(fileTarget);
        }

        return fileTargets;
    }
}