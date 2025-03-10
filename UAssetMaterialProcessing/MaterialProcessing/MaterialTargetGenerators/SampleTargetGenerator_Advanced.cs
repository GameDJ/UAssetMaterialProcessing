using System;
using System.Collections.Generic;

public class ExampleTargetGenerator_Advanced : MaterialTargetGenerator {
    public static List<FileTarget> GenerateTargets() {
        List<FileTarget> fileTargets = new List<FileTarget>();

        // Here I'll show some slightly more advanced topics. 

        string magikCharLocalPathPrefix = "Marvel\\Content\\Marvel\\Characters\\1029\\1029500\\Materials\\10290\\Lobby\\";
        string magikCharSampleFilePrefix = "MI_1029500_10290_";

        // This file is the material for Magik's Eldritch Armor's crown
        FileTarget crownMaterial = new FileTarget(magikCharLocalPathPrefix, magikCharSampleFilePrefix + "Equip_04");

        // We can add a scalar target the same way we do vector ones.
        // This simply sets the value of cubeMapStrength to 5.
        crownMaterial.AddScalarTarget(name => name == "cubeMapStrength", value => 5);

        // (now back to vectors)
        // Earlier we just targeted all vectors with "Color" in the name. We can do that while also including more specific targets.
        // IMPORTANT: The targeting function that succeeds FIRST will be used; therefore make sure to include more-specific targets before more-general ones!

        // So first, we'll define a modification that sets a specific vector to a specific value.
        crownMaterial.AddVectorTarget(name => name == "BasicLineColor", value => [1, 0.8f, 0.5f]);

        // Then, we'll define a more general modification.
        // BasicLineColor should end up using the above modification, while everything else should use the below one.
        crownMaterial.AddVectorTarget(name => name.ToLower().Contains("color"), value => [value[0] / 2f, value[1] / 2f, value[2] / 2f]);

        fileTargets.Add(crownMaterial);

        // Since we'll probably mostly be editing colors, I wrote some methods for working with colors.
        // They're in the "MarvelRivals" file/namespace, in the class ColorOps.

        // First I'll show an example of a basic modification.
        // This is the material for Magik's Eldritch Armor's chest gem.
        FileTarget chestgemMaterial = new FileTarget(magikCharLocalPathPrefix, magikCharSampleFilePrefix + "Equip_05");
        // We'll target colors like usual...
        Func<string, bool> targetColorsFunc = name => name.ToLower().Contains("color");

        // This'll use my function ModifyHSL. It makes it easier to edit an RGB color in a practical manner by converting it to HSL color.
        // HSL = Hue, Saturation, Lightness. I'd recommend using an online HSL color picker to better understand it.
        //      Hue is the color from 0 to 359, where 0 is red, 120 is green, 240 is blue, and at 360 it wraps back around to 0 (red).
        //      Saturation is how strong the hue shows up from white; 0 is pure white/gray, while 255 is pure color (scaling to black if lightness is 0).
        //      Lightness is how strong the hue shows up from black; 0 is pure black, while 255 is pure color (scaling to white if saturation is 0).
        // So for ModifyHSL, you first pass in the rgb float[], then any H, S, and/or L functions you want to perform on it. 
        // (Be aware of the range of each data type!! rgb float[] channels go from 0f to 1f, H is an int from 0 to 359, and S & L are bytes from 0 to 255)
        // Here we'll just pass in a function which sets the hue to 0 (red).
        // (btw, the parameter name in the arrow function doesn't matter; "rgb" is probably more descriptive than "value")
        Func<float[], float[]> setHueRedFunc = rgb => ColorOps.ModifyHSL(rgb, ModifyH: hue => 0);

        // However, there's a problem with just calling this ModifyHSL function; it clamps the value of each rgb channel to [0, 1].
        // Meanwhile, many color channel values in this game go above 1; this is crucial for things like bright flame effects.
        // So, you'll want to wrap calls of ModifyHSL inside the succinctly-named function NormalizeThenFunctionThenUnnormalize().
        // So if the original rgb was [0, 5, 10], this'll change it to [10, 0, 0] (pure red with max intensity of 10 restored).
        Func<float[], float[]> normalizedSetHueRedFunc = rgb => ColorOps.NormalizeThenFunctionThenUnnormalize(rgb, setHueRedFunc);
        chestgemMaterial.AddVectorTarget(targetColorsFunc, normalizedSetHueRedFunc);
        fileTargets.Add(chestgemMaterial);

        // I've also written a more specific hue modification function which I used for transforming gradients.
        // Basically, Magik's Eldritch Armor flames have a blue -> cyan gradient; I wanted to remap this gradient to red -> yellow.
        // So I have a function that takes a source gradient (eg. blue -> cyan), a target gradient (eg. red -> yellow), and a hue which (theoretically)
        //  is somewhere on the source gradient (eg. sky blue), and then it maps it to the equivalent position on the target gradient (eg. orange).
        // Note that hues outside the source gradient will get clamped to its nearest edge; eg. purple would get clamped to blue, which becomes red.

        // I'll show an example on one of the flames on Magik's Eldritch armor:
        string magikVfxLocalPathPrefix = "Marvel\\Content\\Marvel\\VFX\\Materials\\Characters\\1029\\Materials\\";
        string magikSampleVfxFilename = "MI_1029500_Flow_26_001";
        FileTarget flowMaterial = new FileTarget(magikVfxLocalPathPrefix, magikSampleVfxFilename);

        Func<int, int> hueMapper_BlueCyan_to_RedYellow =
            hue => ColorOps.MapHueToGradient(
                240, // blue
                180, // cyan
                false, // "negative" direction in the spectrum
                0, // red
                60, // yellow
                true, // "positive" direction in the spectrum
                hue // hue to map from source to target; eg. 210 will become 30
            );
        // Now we need to pass this hueMapper to ModifyHSL to modify an actual rgb value and not just a hue...
        Func<float[], float[]> hslMapper_BlueCyan_to_RedYellow = rgb => ColorOps.ModifyHSL(rgb, ModifyH: hueMapper_BlueCyan_to_RedYellow);
        // Now, as before, we need to account for high-intensity values by wrapping it in the normalization function
        Func<float[], float[]> preserveIntensityColorMapper_BlueCyan_to_RedYellow = unnormalized_rgb => ColorOps.NormalizeThenFunctionThenUnnormalize(unnormalized_rgb, hslMapper_BlueCyan_to_RedYellow);

        flowMaterial.AddVectorTarget(targetColorsFunc, preserveIntensityColorMapper_BlueCyan_to_RedYellow);
        fileTargets.Add(flowMaterial);

        return fileTargets;
    }
}