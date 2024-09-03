using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector.Editor; // Import Odin Editor namespace
using MageAFK.Items;
using System.Collections.Generic;

namespace MageAFK.Creation
{
    public class ScriptableMigration : OdinEditorWindow
    {

        [MenuItem("Tools/Migrate ArmourData to StatArmourData")]
        public static void Migrate()
        {

            // Find all ArmourData assets
            string[] guids = AssetDatabase.FindAssets("t:ArmourData");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ArmourData oldData = AssetDatabase.LoadAssetAtPath<ArmourData>(path);

                if (oldData == null) continue; // Skip if not loaded properly

                // Determine the new type based on some condition
                // For example, let's assume you're migrating to PlateArmourData
                StatArmourData newData = CreateInstance<StatArmourData>();

                // Copy fields from oldData to newData
                // You will need to implement this based on your data structure
                // For example:
                newData.iD = oldData.iD;
                newData.itemName = oldData.itemName;
                newData.description = oldData.description;
                newData.image = oldData.image;
                newData.grade = oldData.grade;
                newData.mainType = oldData.mainType;
                newData.dropInfo = oldData.dropInfo;
                newData.types = new List<ItemType>(oldData.types);


                // Create new asset
                string newPath = path.Replace("ArmourData", "StatArmourData");
                AssetDatabase.CreateAsset(newData, newPath);

            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Migration completed.");
        }

        private void Migrate<Parent, Child>()
        {



        }

    }
}

