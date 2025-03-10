using UAssetAPI.UnrealTypes;
using UAssetAPI;
using UAssetAPI.Unversioned;
using System.IO;
using ColorHelper;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.ExportTypes;
using System.Collections.Generic;
using System.Threading.Channels;
//using static UAssetModifying;
//using static FileOps;

class MyProcess {
    static EngineVersion engineVersion = EngineVersion.VER_UE5_3;
    // Update these path values to match your own machine
    static Usmap mappings = new Usmap("D:\\Modding\\MarvelRivals\\mappings\\5.3.2-1525091+++depot_marvel+S1_1_release-Marvel.usmap");

    static void Main(string[] args) {
        MaterialWriter UAssetWriter = new MaterialWriter(engineVersion, mappings);

        //UAssetWriter.ModifyAndWrite(ExampleTargetGenerator_Basic.GenerateTargets(), "example-basic");
        //UAssetWriter.ModifyAndWrite(ExampleTargetGenerator_Advanced.GenerateTargets(), "example-advanced");
        //UAssetWriter.ModifyAndWrite(JeffWater.GenerateTargets(), "Jeff-Water-Yellow_P");
        UAssetWriter.ModifyAndWrite(MagikEldritchRed.GenerateTargets(), "Magik-Eldritch-Red-VFX_P");
        //UAssetWriter.ModifyAndWrite(MagikEldritchPurple.GenerateTargets(), "Magik-Eldritch-Purple-VFX_P");
        //UAssetWriter.ModifyAndWrite(MagikEldritchPink.GenerateTargets(), "Magik-Eldritch-Pink-VFX-v1_P");
        //UAssetWriter.ModifyAndWrite(MagikEldritchYellow.GenerateTargets(), "Magik-Eldritch-Yellow-VFX_P");
        //UAssetWriter.ModifyAndWrite(MagikVfxTesting.GenerateTargets(), "Magik-vfxTest-magenta_P");
        //UAssetWriter.ModifyAndWrite(StormGreen.GenerateTargets(), "Storm-vfxMats-green-v1_P");
        //UAssetWriter.ModifyAndWrite(SnowVenom.GenerateTargets(), "SnowVenom-testing-v03-vfxMats-103561_P");
        //UAssetWriter.ModifyAndWrite(StrangePortal.GenerateTargets(), "Strange-portal-green_P");
        //UAssetWriter.ModifyAndWrite(PsylockeButterflies.GenerateTargets(), "Psylocke-butterflies-black-v1_P");
        //UAssetWriter.ModifyAndWrite(BlackPantherBeachVibes.GenerateTargets(), "BlackPantherVFX-BeachVibes-v1_P");

        //FileOps.PrintAllMaterialColorsInDirectory("uassets\\Marvel\\Content\\Marvel\\VFX\\Materials\\Characters\\1029\\Materials\\", engineVersion, mappings);

    }


}


