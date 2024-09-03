// using UnityEngine;
// using UnityEditor;
// using System.Linq;
// using MageAFK.UI;

// [CustomEditor(typeof(IndicatorHandler))]
// public class IndicatorSearchBar : Editor
// {
//   private string searchBarText = "";

//   public override void OnInspectorGUI()
//   {


//     // Search Bar
//     EditorGUILayout.Space();
//     GUILayout.Label("Search Bar", EditorStyles.boldLabel);

//     // Using EditorGUILayout.BeginHorizontal() to have the text field and the button on the same line
//     EditorGUILayout.BeginHorizontal();

//     searchBarText = EditorGUILayout.TextField(searchBarText);

//     if (GUILayout.Button("Search"))
//     {
//       IndicatorHandler component = (IndicatorHandler)target;
//       component.OrganizeListBasedOnString(searchBarText);
//     }

//     EditorGUILayout.EndHorizontal();

//     // Draw default inspector properties
//     DrawDefaultInspector();
//   }
// }
