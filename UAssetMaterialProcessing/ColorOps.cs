using System;
using ColorHelper;


/// <summary>
/// Custom operations for working with color data.<br/><br/>
/// NOTE: When messing with these, be very careful about what kind of data you're working with.<br/>
/// float[] represents RGB arrays where each value is a float from 0 to 1; the I/O of these methods
///  generally uses this since it's what's used by the Unreal assets.<br/>
/// RGB data type uses bytes, instead ranging from 0 to 100.<br/>
/// HSL data type uses bytes for S and L, and for H it uses an int from 0 to 359 (representing the 360-degree color wheel)
/// </summary>
class ColorOps {
    /// <summary>
    /// Sometimes, RGB values can be over 1; this is intentional, though color libraries don't necessarily support it.
    /// Wrap your color modifications in this function to preserve the color's original intensity.
    /// It normalizes the max value to 1, then performs the modification, then un-normalizes based on the original max value.
    /// </summary>
    /// <param name="rgb">float[] with values from 0 to 1</param>
    /// <param name="operation"></param>
    /// <returns>The modified rgb float[] with max intensity preserved.</returns>
    public static float[] NormalizeThenFunctionThenUnnormalize(float[] rgb, Func<float[], float[]> operation) {
        float maxChannelVal;
        // first multiply values by 1000 to ensure we dont get float underflow
        float[] normalized = NormalizeRGB(rgb, out maxChannelVal);
        // Perform operation
        float[] modified = operation(normalized);
        // Un-normalize based on original intensity
        float[] unnormalized = [modified[0] * maxChannelVal, modified[1] * maxChannelVal, modified[2] * maxChannelVal];
        return unnormalized;

    }

    /// <summary>
    /// Takes an RGB float[] and performs H, S, and/or L operations on it.<br/>
    /// Make sure to wrap this function in NormalizeThenFunctionThenUnnormalize() to preserve color intensity
    /// </summary>
    /// <param name="rgb">float[] with values from 0 to 1</param>
    /// <param name="ModifyH"></param>
    /// <param name="ModifyS"></param>
    /// <param name="ModifyL"></param>
    /// <returns>The modified rgb float[]</returns>
    public static float[] ModifyHSL(
        float[] rgb,
        Func<HSL, HSL>? ModifyHSL = null,
        Func<int, int>? ModifyH = null,
        Func<byte, byte>? ModifyS = null,
        Func<byte, byte>? ModifyL = null
    ) {
        // Clamp values to ensure they work properly with ColorHelper.
        // If you want to preserve >1 values, wrap this function call inside a call of NormalizeThenFunctionThenUnnormalize()
        float clampedR = Clamp(rgb[0]);
        float clampedG = Clamp(rgb[1]);
        float clampedB = Clamp(rgb[2]);

        // Short-circuit:
        // Original color is a shade of gray, and we're not targeting L (which is the only HSL channel gray uses)
        if ((ModifyHSL == null && ModifyL == null) && clampedR == clampedG && clampedG == clampedB) {
            return [clampedR, clampedG, clampedB];
        }

        // Convert given RGB to HSL
        RGB colorRGB = new RGB((byte)(Math.Round(clampedR * 255)), (byte)Math.Round(clampedG * 255), (byte)Math.Round(clampedB * 255));
        HSL colorHSL = ColorConverter.RgbToHsl(colorRGB);

        Console.WriteLine($"\t\t\tHSL: {colorHSL.H} {colorHSL.S} {colorHSL.L}");

        if (ModifyHSL != null) {
            colorHSL = ModifyHSL(colorHSL);
            Console.WriteLine($"{colorHSL.H} {colorHSL.S} {colorHSL.L}");
        } else {
            if (ModifyH != null) {
                colorHSL.H = (int)ModifyH(colorHSL.H);
            }
            if (ModifyS != null) {
                colorHSL.S = (byte)ModifyS(colorHSL.S);
            }
            if (ModifyL != null) {
                colorHSL.L = (byte)ModifyL(colorHSL.L);
            }
        }

        Console.WriteLine($"\t\t\tHSL: {colorHSL.H} {colorHSL.S} {colorHSL.L}");

        // convert back to RGB
        RGB targetRGB = ColorConverter.HslToRgb(colorHSL);
        return [(float)targetRGB.R / 255f, (float)targetRGB.G / 255f, (float)targetRGB.B / 255f];
    }

    /// <summary>
    /// Map a hue from its position within a source gradient to the equivalent relative position in a target gradient.<br/>
    /// For example, if the source is green -> blue, and the target is red -> yellow, and the hueToMap is cyan (halfway between green and blue), then the output will be orange (halfway between red and yellow).<br/>
    /// <br/>NOTE: Hues outside the source spectrum get clamped to its nearest end.<br/>
    /// For example:<br/>
    ///     sourceStartHue = 180 (cyan)<br/>
    ///     sourceEndHueEnd = 240 (blue)<br/>
    ///     sourceDirIsPositive = true<br/>
    ///     targetStartHue = 0 (red)<br/>
    ///     targetEndHue = 60 (yellow)<br/>
    ///     targetDirIsPositive = true;<br/>
    ///     hueToMap = 120 (green)<br/>
    /// hueToMap's 120 (green) will get clamped to 180 (cyan), which is then converted into 0 (red).<br/>
    /// <br/>Gradient direction refers to direction in the rainbow; for example, red -> yellow is positive, while yellow -> red is negative.<br/>
    /// So for example, (start=240, end=180, directionIsPositive=false) means a gradient going from blue to cyan.<br/>
    /// If you did the same with directionIsPositive=true, it would go up and around the spectrum like blue->red->yellow->cyan.
    /// </summary>
    /// <param name="sourceStartHue">(0 to 359) Source gradient's starting hue</param>
    /// <param name="sourceEndHue">(0 to 359) Source gradient's ending hue</param>
    /// <param name="sourceDirIsPositive">Direction in the rainbow of source gradient</param>
    /// <param name="targetStartHue"></param>
    /// <param name="targetEndHue"></param>
    /// <param name="targetDirIsPositive"></param>
    /// <param name="hueToMap">(0 to 359) hue value to convert from position in source gradient to target gradient</param>
    /// <returns>A hue (from 0 to 359) on the target gradient</returns>
    public static int MapHueToGradient(
        int sourceStartHue,
        int sourceEndHue,
        bool sourceDirIsPositive,
        int targetStartHue,
        int targetEndHue,
        bool targetDirIsPositive,
        int hueToMap
    ) {
        // first we're going to unwrap our gradients (if necessary) into one continuous scale for easier math
        if (sourceDirIsPositive && sourceEndHue < sourceStartHue) {
            sourceEndHue += 360;
        } else if (!sourceDirIsPositive && sourceEndHue > sourceStartHue) {
            sourceStartHue += 360;
        }
        if (targetDirIsPositive && targetEndHue < targetStartHue) {
            targetEndHue += 360;
        } else if (!targetDirIsPositive && targetEndHue > targetStartHue) {
            targetStartHue += 360;
        }
        // Clamp hue to source gradient, making sure to check whether it got left behind when one end jumped up 360
        if ((sourceDirIsPositive && (hueToMap < sourceStartHue || hueToMap > sourceEndHue)) ||
            (!sourceDirIsPositive && (hueToMap > sourceStartHue || hueToMap < sourceEndHue))) {
            // Hue is not within bounds

            if ((sourceDirIsPositive && hueToMap + 360 > sourceStartHue && hueToMap + 360 < sourceEndHue) ||
            (!sourceDirIsPositive && hueToMap + 360 < sourceStartHue && hueToMap + 360 > sourceEndHue)) {
                // hue was supposed to be inside but got left behind when we moved one side by 360
                hueToMap += 360;
            } else {
                // Neither hue nor adjusted hue are within bounds; we must clamp it to the nearest bound
                int hueDistFromStart = Math.Abs(hueToMap - sourceStartHue);
                int hueDistFromEnd = Math.Abs(hueToMap - sourceEndHue);
                int hue360DistFromStart = Math.Abs(hueToMap + 360 - sourceStartHue);
                int hue360DistFromEnd = Math.Abs(hueToMap + 360 - sourceEndHue);
                int minDistance = Math.Min(hueDistFromStart, Math.Min(hueDistFromEnd, Math.Min(hue360DistFromStart, hue360DistFromEnd)));
                if (minDistance == hueDistFromStart || minDistance == hue360DistFromStart) {
                    //hueToMap = sourceStartHue;
                    return targetStartHue % 360;
                } else {
                    //hueToMap = sourceEndHue;
                    return targetEndHue % 360;
                }
            }

        }
        // Now we dont need to worry about wraparound

        // Determine relative position of hueToMap on the source gradient
        int sourceDistFromStartToHue = Math.Abs(hueToMap - sourceStartHue);
        int sourceDistFromStartToEnd = Math.Abs(sourceEndHue - sourceStartHue);
        float huePositionRatio = (float)sourceDistFromStartToHue / (float)sourceDistFromStartToEnd;

        // Place the position onto the target gradient
        int targetDistFromStartToEnd = Math.Abs(targetEndHue - targetStartHue);
        int hueTargetPosition = targetStartHue + (int)Math.Round(huePositionRatio * targetDistFromStartToEnd);
        // Wrap back around to the base spectrum
        hueTargetPosition %= 360;

        return hueTargetPosition % 360;
    }

    /// <param name="value"></param>
    /// <returns>The provided float, clamped from [0,1]</returns>
    private static float Clamp(float value) {
        //return ClampHelper.Clamp(value, 0, 1);
        return Math.Clamp(value, 0, 1);
    }

    public static float[] NormalizeRGB(float[] rgb) {
        float[] normalized = NormalizeRGB(rgb, out _);
        return normalized;
    }
    public static float[] NormalizeRGB(float[] rgb, out float factor) {
        float maxChannelVal = Math.Max(Math.Max(rgb[0], rgb[1]), rgb[2]);
        // Normalize max value to 1
        float[] normalized = rgb;
        if (maxChannelVal != 0) {
            normalized = [rgb[0] / maxChannelVal, rgb[1] / maxChannelVal, rgb[2] / maxChannelVal];
            factor = maxChannelVal;
        } else {
            factor = 1;
        }

        return normalized;
    }

    public static HSL RGBtoHSL(float[] rgb) {
        float[] normalized = NormalizeRGB(rgb);
        RGB colorRGB = new RGB((byte)(Math.Round(normalized[0] * 255)), (byte)Math.Round(normalized[1] * 255), (byte)Math.Round(normalized[2] * 255));
        HSL colorHSL = ColorConverter.RgbToHsl(colorRGB);

        return colorHSL;
    }

    public static string GetColorName(float[] rgb) {
        float[] normalized = NormalizeRGB(rgb);
        RGB colorRGB = new RGB((byte)(Math.Round(normalized[0] * 255)), (byte)Math.Round(normalized[1] * 255), (byte)Math.Round(normalized[2] * 255));
        HSL colorHSL = ColorConverter.RgbToHsl(colorRGB);

        return GetColorName(colorHSL);
    }

    public static string GetColorName(HSL hsl) {
        string retVal = "";
        if (hsl.L == 0) {
            return "black";
        }
        if (hsl.L == 100) {
            return "white";
        }
        if (hsl.L <= 33) {
            retVal += "dark";
        } else if (hsl.L > 66) {
            retVal += "light";
        }
        if (hsl.S == 0) {
            return retVal + "gray";
        }
        string[] colors = ["red", "orange", "yellow", "lime", "green", "mint", "cyan", "azure", "blue", "purple", "magenta", "rose"];
        // Hue is adjusted so that 345 (the start of red) becomes 0, then we will check increments of 30
        int adjustedHue = (hsl.H + 15) % 360;
        for (int i = 0; i < colors.Length; i++) {
            if (adjustedHue < i*30+30) {
                return retVal + colors[i];
            }
        }
        return "error";
    }

    /// <summary>
    /// Map a value, originally on a linear scale from 0 to 1, to a new scale with the given pivot "key" value.<br/>
    /// Basically you change the points of the linear curve from [0, 0.5, 1] to [0, value, 1].<br/>
    /// In other words, use this to modify the "center point" of a set of values.<br/>
    /// Example: <br/>
    /// Beginning set: [0,  0.1,  0.4,  0.6,  0.9,  1]<br/>
    /// Call this function on each value, with a newPivot of 0.25<br/>
    /// Resulting set: [0,  0.05, 0.2,  0.4,  0.8,  1]<br/>
    /// Or call the function with a newPivot of 0.25...<br/>
    /// Resulting set: [0,  0.15, 0.6,  0.8,  0.95, 1]<br/>
    /// <br/>
    /// Remember to convert to and from a float from 0 to 1 if you're starting with eg. a byte
    /// </summary>
    /// <param name="value"></param>
    /// <param name="newPivot"></param>
    /// <param name="oldPivot">Untested default parameter; change to compute the old pivot as being somewhere besides the center</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static float ShiftValueInRangeByPivot(float value, float newPivot, float oldPivot = 0.5f) {
        // error check
        if (value < 0 || value > 1) {
            throw new ArgumentOutOfRangeException("value is out of range");
        }
        if (oldPivot < 0 || oldPivot > 1 ) {
            throw new ArgumentOutOfRangeException("oldPivot is out of range");
        }
        if (newPivot < 0 || newPivot > 1) {
            throw new ArgumentOutOfRangeException("newPivot is out of range");
        }

        // remap
        if (value <= oldPivot) {
            // value is between min and original pivot

            // Determine relative position within segment
            float relPos = value / oldPivot;
            // Scale the value onto the new segment
            float absPosOnNewSegment = relPos * newPivot;
            return absPosOnNewSegment;
        }
        else {
            // value is between original pivot and max

            // Shift placement down to pretend oldPivot is 0
            float absPosOnOldSegment = value - oldPivot;
            // Determine relative position within segment
            float relPos = absPosOnOldSegment / (1 - oldPivot);
            // Scale the value onto the new segment
            float absPosOnNewSegment = relPos * (1 - newPivot);
            // Shift it back up based on the segment's start position
            float finalPosition = absPosOnNewSegment + newPivot;
            return finalPosition;
        }
    }
}
