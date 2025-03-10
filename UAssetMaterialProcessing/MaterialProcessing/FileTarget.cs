using System;
using System.Collections.Generic;

public struct FileTarget {
    public string localPathPrefix;
    public string name;
    public List<Tuple<Func<string, bool>, Func<float, float>>>? scalarTargets;
    public List<Tuple<Func<string, bool>, Func<float[], float[]>>>? vectorTargets;

    public FileTarget(string localPathPrefix, string name) {
        this.localPathPrefix = localPathPrefix;
        this.name = name;
    }

    public void AddScalarTarget(Func<string, bool> targetFunc, Func<float, float> modifyFunc) {
        if (this.scalarTargets == null) {
            this.scalarTargets = new List<Tuple<Func<string, bool>, Func<float, float>>>();
        }
        this.scalarTargets.Add(new(targetFunc, modifyFunc));
    }

    public void AddVectorTarget(Func<string, bool> targetFunc, Func<float[], float[]> modifyFunc) {
        if (this.vectorTargets == null) {
            this.vectorTargets = new List<Tuple<Func<string, bool>, Func<float[], float[]>>>();
        }
        this.vectorTargets.Add(new(targetFunc, modifyFunc));
    }
}