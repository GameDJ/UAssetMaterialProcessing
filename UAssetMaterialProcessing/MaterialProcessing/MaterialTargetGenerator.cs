using System;
using System.Collections.Generic;

public interface MaterialTargetGenerator {
    public abstract static List<FileTarget> GenerateTargets();
}
