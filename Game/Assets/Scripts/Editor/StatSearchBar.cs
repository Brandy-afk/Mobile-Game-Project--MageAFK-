// using System.Collections;
// using System.Collections.Generic;
// using MageAFK.Stats;
// using UnityEditor;
// using UnityEngine;


// [CustomEditor(typeof(StatInformation))]
// public class StatSearchBar : Editor
// {
//   private string searchBarText = "";

//   public override void OnInspectorGUI()
//   {
//     // Search Bar
//     EditorGUILayout.Space();
//     GUILayout.Label("Search Bar", EditorStyles.boldLabel);

//     EditorGUILayout.BeginHorizontal();

//     searchBarText = EditorGUILayout.TextField(searchBarText);
//     if (GUILayout.Button("Search"))
//     {
//       StatInformation component = (StatInformation)target;
//       component.OrganizeStatInfo(searchBarText);
//     }

//     EditorGUILayout.EndHorizontal();

//     if (GUILayout.Button("Pack stat info list"))
//     {
//       StatInformation component = (StatInformation)target;
//       component.PackList();
//     }




//     DrawDefaultInspector();
//   }


// }
