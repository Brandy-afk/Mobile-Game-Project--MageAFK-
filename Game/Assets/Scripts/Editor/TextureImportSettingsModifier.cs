// using UnityEngine;
// using UnityEditor;
// using System.IO;

// public class TextureImportSettingsModifier : EditorWindow
// {
//   private string rootPath = "Assets/MyAssets/Enemies";

//   [MenuItem("Tools/Modify Texture Import Settings")]
//   public static void ShowWindow()
//   {
//     EditorWindow.GetWindow<TextureImportSettingsModifier>("Texture Settings Modifier");
//   }

//   private void OnGUI()
//   {
//     GUILayout.Label("Root Directory for Modification", EditorStyles.boldLabel);
//     rootPath = EditorGUILayout.TextField("Root Path", rootPath);

//     if (GUILayout.Button("Modify Settings"))
//     {
//       ChangeImportSettings(rootPath);
//       this.Close();
//     }
//   }

//   private void ChangeImportSettings(string rootPath)
//   {
//     string[] fileEntries = Directory.GetFiles(rootPath);
//     foreach (string fileName in fileEntries)
//     {
//       string ext = Path.GetExtension(fileName).ToLower();
//       if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".tga" || ext == ".bmp")
//       {
//         string assetPath = fileName.Replace(Application.dataPath, "Assets");
//         TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

//         if (textureImporter != null)
//         {
//           // textureImporter.filterMode = FilterMode.Point;
//           // textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
//           textureImporter.spritePixelsPerUnit = 16;
//           textureImporter.spriteImportMode = SpriteImportMode.Multiple;
//           AssetDatabase.ImportAsset(assetPath);
//         }
//       }
//     }

//     // Recurse into subdirectories.
//     string[] subdirectoryEntries = Directory.GetDirectories(rootPath);
//     foreach (string subdirectory in subdirectoryEntries)
//     {
//       ChangeImportSettings(subdirectory);
//     }
//   }
// }
