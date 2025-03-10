using UAssetAPI.UnrealTypes;

class RichCurve {
    public List<FRichCurveKey>[] colorCurves;
    public static string[] channelMap = ["R", "G", "B", "A"];

    public RichCurve() {
        colorCurves = new List<FRichCurveKey>[4];
        for (int i = 0; i < 4; i++) {
            colorCurves[i] = new List<FRichCurveKey>();
        }
    }

    public bool AddRgbaKey(float time, float[] rgba) {
        for (int i = 0; i < 4; i++) {
            // should check if key already exists here for this channel, and delete it (or give error)
            //

            FRichCurveKey newKey = new FRichCurveKey();
            newKey.Time = time;
            newKey.Value = rgba[i];
            colorCurves[i].Add(newKey);
        }
        return true;
    }

    public bool AddKey(int channel, FRichCurveKey newKey) {
        if (GetKey(channel, newKey.Time) == null) {
            colorCurves[channel].Add(newKey);
            return true;
        }
        return false;
    }

    public FRichCurveKey? GetKey(int channel, float time) {
        var findResult = colorCurves[channel].Find(curveKey => curveKey.Time == time);
        if (findResult.Equals(default(FRichCurveKey))) {
            // match not found
            //throw new KeyNotFoundException($"Key does not exist in channel {channel} at time {time}");
            return null;
        }
        return findResult;
    }

    public bool CheckKeyExists(int channel, float time) {
        try {
            GetKey(channel, time);
        } catch (KeyNotFoundException) {
            return false;
        }
        return true;
    }

    /// <summary>
    /// PRECONDITION: Channel keys are sorted according to increasing Time
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public float GetChannelValueAtTime(int channel, float time) {
        // Determine the value of a channel at a certain time, given the keys

        // This channel must have at least 1 key or else this request makes no sense
        if (colorCurves[channel].Count == 0) {
            throw new KeyNotFoundException($"Attempted to get value at time {time} of channel {channel} which has no keys");
        }

        // Short-circuit: If only 1 key, that's our value
        if (colorCurves[channel].Count == 1) {
            return colorCurves[channel][0].Value;
        }


        // LINEAR INTERPOLATION

        // Find the closest key above and/or below the given time
        FRichCurveKey? lowerKey = null;
        FRichCurveKey? upperKey = null;
        foreach (FRichCurveKey key in colorCurves[channel]) {
            if (key.Time == time) {
                // exact match found; can return it directly
                return key.Value;
            } else if (key.Time < time) {
                // set this key as the lower key (if multiple keys are lower, the closest one will end up as this)
                lowerKey = key;
            } else if (upperKey == null) {
                // if upper key hasn't yet been set, set this key to it then quit loop
                upperKey = key;
                break;
            }
        }

        // If given time was not in-between two keys, it has same value as the closest key
        // We already know there must be more than 1 total keys, so both of these being null is unforeseen
        if (lowerKey == null) {
            return upperKey!.Value.Value;
        }
        if (upperKey == null) {
            return lowerKey!.Value.Value;
        }

        // Calculate the interpolated value
        float timeDiff = upperKey.Value.Time - lowerKey.Value.Time;
        float valueDiff = upperKey.Value.Value - lowerKey.Value.Value;
        float adjustedTime = time - lowerKey.Value.Time;
        float timeRatio = adjustedTime / timeDiff;
        return valueDiff * timeRatio + lowerKey.Value.Value;
    }

    public float[] GetColorAtTime(float time) {
        // error checking
        if (time < 0 || time > 1f) {
            throw new ArgumentOutOfRangeException();
        }
        float[] rgba = new float[4];
        for (int i = 0; i < 4; i++) {
            List<FRichCurveKey> channelKeys = colorCurves[i];
            if (channelKeys.Count == 0) {
                throw new Exception($"{channelMap[i]} channel has no set keys");
            }
            rgba[i] = GetChannelValueAtTime(i, time);
        }

        return rgba;
        //return new FLinearColor(rgba[0], rgba[1], rgba[2], rgba[3]);
    }

    public void PrintKeys() {
        for (int i = 0; i < 4; i++) {
            Console.WriteLine(channelMap[i]);
            for (int j = 0; j < colorCurves[i].Count; j++) {
                FRichCurveKey key = colorCurves[i][j];
                Console.WriteLine("\t{0, 2}:  T: {1, 14}\tV:{2, 14}", j, key.Time, key.Value);
            }
        }
        Console.WriteLine();
    }
}
