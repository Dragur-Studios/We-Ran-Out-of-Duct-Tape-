using UnityEngine;

public static class GOAPUtil
{
    public static Vector3 StringToVector3(string s, Vector3 fallback = default)
    {
        if (string.IsNullOrWhiteSpace(s))
            return fallback;

        var parts = s.Split(',');
        if (parts.Length != 3)
            return fallback;

        if (float.TryParse(parts[0].Trim(), out var x) &&
            float.TryParse(parts[1].Trim(), out var y) &&
            float.TryParse(parts[2].Trim(), out var z))
        {
            return new Vector3(x, y, z);
        }

        return fallback;
    }
}
