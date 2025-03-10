using System;
using System.IO;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Versions;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.GameTypes.NetEase.MAR.Encryption.Aes;
using CUE4Parse.Encryption.Aes;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.MappingsProvider;
using CUE4Parse.GameTypes.FF7.Assets.Exports;
using CUE4Parse.Compression;
using Newtonsoft.Json;

public class MasterMaterialTesting
{
	public MasterMaterialTesting()
	{
	}
    static void MasterMaterialTestingfunc() {
        //string gameDir = "D:\\SteamLibrary\\steamapps\\common\\MarvelRivals\\MarvelGame";
        //string inputAssetsDir = "D:\\Modding\\MarvelRivals\\repos\\UAssetMaterialProcessing\\UAssetMaterialProcessing\\bin\\Debug\\net8.0\\uassets";
        //string stormMMdir = "Marvel\\Content\\Marvel\\VFX\\Materials\\Characters\\1015\\MasterMaterials";
        //string packageRelDir = "Marvel\\Content\\Paks";
        //string packageName = "pakchunkCharacter-Windows";
        //string stormMMdir = "uassets\\Marvel\\Content\\Marvel\\VFX\\Materials\\Characters\\1015\\MasterMaterials\\";
        //string targetMM = "M_Lightning_6_601.uasset";

        //var versionContainer = new VersionContainer();

        //var provider = new DefaultFileProvider($"{gameDir}\\{packageRelDir}", SearchOption.TopDirectoryOnly, false, new VersionContainer(EGame.GAME_UE5_3));
        //provider.Initialize(); // will scan the archive directory for supported file extensions

        ////var obj = provider.LoadObject($"{gameDir}\\{packageRelDir}\\{packageName}.{stormMMdir}\\{targetMM}"); // {GAME}/Content/Folder1/Folder2/PackageName.ObjectName
        ////var obj = provider.LoadObject($"D:\\Applications2\\FModel\\Output\\Exports\\Marvel\\Content\\Marvel\\VFX\\Materials\\Characters\\1015\\MasterMaterials\\M_Lightning_6_601.uasset"); // {GAME}/Content/Folder1/Folder2/PackageName.ObjectName
        ////var objJson = JsonConvert.SerializeObject(obj, Formatting.Indented);
        ////Console.WriteLine(objJson);

        ////var allObjects = provider.LoadAllObjects($"{gameDir}\\{packageRelDir}\\{packageName}"); // {GAME}/Content/Folder1/Folder2/PackageName.uasset
        //var allObjects = provider.LoadAllObjects($"{stormMMdir}\\{targetMM}"); // {GAME}/Content/Folder1/Folder2/PackageName.uasset

        //const string _gameDirectory = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\MarvelRivals\\MarvelGame\\Marvel\\Content\\Paks";
        const string _pakDir = "D:\\SteamLibrary\\steamapps\\common\\MarvelRivals\\MarvelGame\\Marvel\\Content\\Paks";
        const string _aesKey = "0x0C263D8C22DCB085894899C3A3796383E9BF9DE0CBFB08C9BF2DEF2E84F29D74";
        const string _mappingsPath = "D:\\Modding\\MarvelRivals\\mappings\\5.3.2-1525091+++depot_marvel+S1_1_release-Marvel.usmap";

        //const string _mapping = "C:\\Users\\flavi\\Downloads\\MarvelRivals-API\\MarvelRivals-API\\data-extractor\\data-extractor\\assets\\mapping.usmap";

        //Log.Logger = new LoggerConfiguration().WriteTo.Console(theme: AnsiConsoleTheme.Literate).CreateLogger();

        var oodlePath = Path.Combine("D:/", OodleHelper.OODLE_DLL_NAME);
        if (!File.Exists(oodlePath)) {
            OodleHelper.DownloadOodleDll(oodlePath);
        }
        OodleHelper.Initialize(oodlePath);


        var provider = new DefaultFileProvider(_pakDir, SearchOption.TopDirectoryOnly, true,
            new VersionContainer(EGame.GAME_MarvelRivals));
        provider.CustomEncryption = MarvelAes.MarvelDecrypt;
        provider.MappingsContainer = new FileUsmapTypeMappingsProvider(_mappingsPath);
        provider.Initialize(); // will scan local files and read them to know what it has to deal with (PAK/UTOC/UCAS/UASSET/UMAP)
        provider.SubmitKey(new FGuid(), new FAesKey(_aesKey)); // decrypt basic info (1 guid - 1 key)

        provider.LoadLocalization(ELanguage.English); // explicit enough
        //Console.WriteLine($"Files count {provider.Files.Count}");
        //var files = provider.Files.Keys.Where(key => key.ToLower().Contains("vfx") && key.ToLower().Contains("master") && key.ToLower().Contains("1015") && key.ToLower().Contains("lightning"));
        //foreach (var item in files) {
        //    Console.WriteLine(item);
        //}
        //for (int i=30000; i <= 80000; i += 500) {
        //    Console.WriteLine($"{i} {provider.Files.Keys.ToArray()[i]}");

        //}
        //Console.WriteLine(provider.Files["marvel/content/marvel/ui/textures/show/spray/img_spray_41036204.uasset"].Read());

        //Console.WriteLine(provider.Files["marvel/content/marvel/vfx/materials/characters/1015/mastermaterials/m_lightning_6_601.uasset"].Read());

        var obj = provider.LoadObject("marvel/content/marvel/vfx/materials/characters/1015/mastermaterials/m_lightning_6_601.m_lightning_6_601");
        var variantJson = JsonConvert.SerializeObject(obj, Formatting.Indented);
        Console.WriteLine(variantJson);
        //foreach (var prop in obj.Properties) {
        //    Console.WriteLine(prop);
        //}

    }
}
