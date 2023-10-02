using System.IO;
using UnityEngine;

public static class ImageLoader
{
    private static readonly string[] ImageFormats = new string[] { ".jpg", ".png", ".webp" };

    public static Texture2D TryLoadImage(string filename, string extension)
    {
        string target = null;

        foreach (var format in ImageFormats)
        {
            if (File.Exists(filename.Replace(extension, format)))
            {
                target = filename.Replace(extension, format);
                break;
            }
        }

        if (target == null)
            return null;

        var bytes = File.ReadAllBytes(target);

        Texture2D img = new Texture2D(16, 16);
        img.LoadImage(bytes);
        img.Apply();

        return img;
    }
}
