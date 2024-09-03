using UnityEditor;
using UnityEngine;

public class ChangeSpritePivotEditor : ScriptableWizard
{
    public Vector2 defaultPivotPoint = new Vector2(0.48f, 0.52f);
    public Vector2 attackPivotPoint = new Vector2(0.39f, 0.384f);

    [MenuItem("Custom/Change Sprite Pivot")]
    static void CreateWizard()
    {
        DisplayWizard<ChangeSpritePivotEditor>("Change Sprite Pivot", "Change Pivot");
    }

    void OnWizardCreate()
    {
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets/Imported Assets/Enemies/Humans/KingdomPack" });

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter ti = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            
            if (ti != null)
            {
                ti.isReadable = true;
                ti.spriteImportMode = SpriteImportMode.Single;

                // Get the sprites in the texture
                Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                foreach (Object sprite in sprites)
                {
                    if (sprite is Sprite)
                    {
                        Debug.Log("Sprite Name: " + sprite.name);

                        if (sprite.name.StartsWith("attack"))
                        {
                            ti.spritePivot = attackPivotPoint;
                            AssetDatabase.ImportAsset(assetPath);
                        }
                    }
                }
            }
        }
    }
}
