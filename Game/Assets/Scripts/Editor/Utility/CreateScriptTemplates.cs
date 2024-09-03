
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace MageAFK.Creation
{

  public static class CreateScriptTemplates
  {

    private static Dictionary<TemplateTypes, string> pathing = new Dictionary<TemplateTypes, string>()
    {
      {TemplateTypes.Enemy, "Assets/ScriptTemplates/CustomEnemy.cs.txt"},
      {TemplateTypes.SpellProjectile, "Assets/ScriptTemplates/SpellProjectile.cs.txt"},
      {TemplateTypes.Spell, "Assets/ScriptTemplates/Spell.cs.txt"}
    };


    #region Manual
    [MenuItem("Assets/Create/Script Creation/EnemyScripts/Basic", priority = 41)]
    public static void CreateEnemyScript()
    {
      string location = "Assets/ScriptTemplates/CustomEnemy.cs.txt";

      ProjectWindowUtil.CreateScriptAssetFromTemplateFile(location, "NewEnemy");
    }

    #endregion


    #region Auto
    public static void CreateScript(string scriptName, string savePath, TemplateTypes type)
    {
      string templatePath = pathing[type];

      string actualSavePath = Path.Combine(savePath, $"{scriptName}.cs");
      EditorUtility.CreateFolderIfNeeded(savePath);


      // Use a method to process the template and save the new script
      ProcessTemplateAndSave(templatePath, actualSavePath, scriptName);
    }

    private static void ProcessTemplateAndSave(string templatePath, string savePath, string scriptName)
    {
      string templateContent = File.ReadAllText(templatePath);

      // Replace placeholders in templateContent if any
      string scriptContent = templateContent.Replace("#SCRIPTNAME#", scriptName);

      File.WriteAllText(savePath, scriptContent);
    }

    #endregion

  }

  public enum TemplateTypes
  {
    Spell,
    Enemy,
    SpellProjectile

  }


}


