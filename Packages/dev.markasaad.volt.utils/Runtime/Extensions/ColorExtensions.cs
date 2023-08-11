using System.Security.Cryptography;
using System.Text;

using UnityEngine;

public static class ColorExtensions {
    public static Color RandomColor(string seed) {
        using var md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(seed));

        return new Color((float)hash[0] / 255, (float)hash[1] / 255, (float)hash[2] / 255);
    }
}