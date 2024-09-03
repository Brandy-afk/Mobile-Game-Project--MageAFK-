using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.Spells
{
    public abstract class Ultimate : Spell
    {
        public static bool placableMode = false;
        public static int ultTimeKey = -1;
        protected static int uses;

        [TabGroup("Ultimate"), PreviewField]
        public Sprite visual;
        [TabGroup("Ultimate")]
        public string tip;

        
    }

    public interface IPlacableUlt
    {
        public int OnPlaced(Vector2 pos);

    }
}
