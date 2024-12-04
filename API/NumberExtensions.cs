using System;

namespace ZenithQoL.API;

public static class NumberExtensions {
    public static float roundTo(this float num, float to) {
        if (to <= 1f) {
            num = MathF.Floor(num) + (MathF.Round((num - MathF.Floor(num)) * (1f / to)) * to);
        }
        else {
            num = MathF.Round(num / to) * to;
        }

        return num;
    }
}