using System.Collections.Generic;
using System.Linq;
using Sirenix;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MageAFK.Management
{
    public class UIController : SerializedMonoBehaviour
    {
        [SerializeField, Tooltip("All UI data scriptables.")] private Dictionary<DataType, ScriptableObject> uiData;
        public static UIController Instance;

        private void Awake() => Instance = this;

        public IData<T> ReturnDataObject<T>(DataType type)
        {
            try
            {
                return uiData[type] as IData<T>;
            }
            catch (KeyNotFoundException)
            {
                Debug.LogError($"Key Error : Return object of {type}");
            }
            return default;
        }

        public void Save<T>(DataType type)
        {
            var obj = ReturnDataObject<T>(type);
            if (obj != null)
                SaveManager.Save(obj, type);
            else
                Debug.LogError($"Reference Error : saving data on {type}");

        }
    }
}
