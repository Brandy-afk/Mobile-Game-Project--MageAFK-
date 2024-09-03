// using System.Collections;
// using System.Collections.Generic;
// using MageAFK.Management;
// using UnityEngine;
// using System.Linq;

// //TODO Not currently in use
// namespace MageAFK.Items
// {
//   public class MissionHandler
//   {

//     private static MissionHandler _instance;
//     public static MissionHandler Instance
//     {
//       get
//       {
//         if (_instance == null)
//         {
//           _instance = new MissionHandler();
//         }
//         return _instance;
//       }
//     }

//     private ItemDataBase itemDataBase;


//     public Mission ReturnMission(ItemGrade grade)
//     {
//       if (itemDataBase == null) { itemDataBase = ItemDataBase.Instance; }

//       // ItemData[] itemList = itemDataBase.ReturnPartList(grade);

//       var sortedList = itemList.Where(item => item.ReturnRecipeState()).ToList();

//       Mission newMission = new Mission();
//       newMission.grade = grade;
//       for (int i = 0; i < 3; i++)
//       {
//         bool addedIngredient = false;
//         int random = UnityEngine.Random.Range(0, sortedList.Count);
//         ItemLevel level = sortedList[random].isUpgradable ? (ItemLevel)(Random.Range(0f, 5f)) : (ItemLevel)5;

//         Ingredient ingredient = new Ingredient(new ItemWithLevel(sortedList[random], level), 1);

//         if (newMission.input == null)
//         {
//           newMission.input = new List<Ingredient>();
//           newMission.input.Add(ingredient);
//           continue;
//         }


//         foreach (Ingredient g in newMission.input)
//         {
//           if (g.info.key == ingredient.info.key)
//           {
//             g.quantity += 1;
//             addedIngredient = true;
//             break;
//           }
//         }


//         if (addedIngredient == true) { continue; }
//         newMission.input.Add(ingredient);
//       }

//       return newMission;

//     }


//   }


//   public class Mission
//   {
//     public List<Ingredient> input;
//     public ItemGrade grade;
//   }

// }
