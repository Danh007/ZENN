using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, "colorramp")]
public class ColorRampImporter : ScriptedImporter
{
    public Gradient gradient = new Gradient();
    public int width = 256;
    public FilterMode filterMode = FilterMode.Bilinear;
    public bool isSRGB = true;

    public override void OnImportAsset(AssetImportContext ctx)
    {
        width = Mathf.Max(2, width);
        Texture2D tex = new Texture2D(width, 1, TextureFormat.RGBA32, false, !isSRGB);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = filterMode;

        Color[] pixels = new Color[width];
        for (int i = 0; i < width; i++)
        {
            float t = i / (float)(width - 1);
            pixels[i] = gradient.Evaluate(t);
        }

        tex.SetPixels(pixels);
        tex.Apply();

        ctx.AddObjectToAsset("MainTex", tex);
        ctx.SetMainObject(tex);
    }

    [MenuItem("Assets/Create/Color Ramp", priority = 301)]
    private static void CreateNewColorRamp()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path)) path = "Assets";
        else if (!Directory.Exists(path)) path = Path.GetDirectoryName(path);

        string assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(path, "NewColorRamp.colorramp"));
        File.WriteAllText(assetPath, "");
        AssetDatabase.Refresh();
    }
}
