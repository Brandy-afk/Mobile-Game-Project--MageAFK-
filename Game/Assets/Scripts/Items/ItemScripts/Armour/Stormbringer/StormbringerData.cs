
using System.Collections.Generic;
using MageAFK.AI;
using MageAFK.Combat;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Pooling;
using MageAFK.Spells;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.Items
{
    [CreateAssetMenu(fileName = "Stormbringer", menuName = "Items/ItemData/Equippable/Stormbringer")]
    public class StormBringerData : ArmourData, IPrefabReturner
    {
        [BoxGroup("Stormbringer")]
        public GameObject prefab;

        [BoxGroup("Stormbringer")]
        public int[] maxTarget = new int[4];

        [BoxGroup("Stormbringer")]
        public float[] damage = new float[4];


        public override string ReturnArmourInfo(ItemLevel level, string highlightHex)
        {
            int intLevel = (int)level;
            string format = $"When you damage enemies with a <color=#{{2}}>storm</color> spell, lightning will arc between up to <color=#{{2}}>{{0}}</color> enemies doing <color=#{{2}}>{{1}}</color> damage.";
            return string.Format(format, maxTarget[intLevel], damage[intLevel], highlightHex);
        }

        public GameObject ReturnPrefab() => prefab;
    }

    public class StormBringer : Armour, ISpellCollisionEvent
    {
        private readonly float damage;
        private readonly int maxTargets;

        public StormBringer(StormBringerData data, ItemLevel level) : base(data, level)
        {
            int integerLevel = (int)level;
            damage = data.damage[integerLevel];
            maxTargets = data.maxTarget[integerLevel];
        }

        public void HandleCollision(Spell spell, NPEntity entity, ref float damage, ref float mod, bool isCrit, bool isPierce, bool isEffect)
        {
            if (spell.element == SpellElement.Storm)
            {
                var newProc = ServiceLocator.Get<ItemPooler>().Get(ItemIdentification.Stormbringer);
                newProc.transform.position = entity.transform.position;
                var newTracker = new HashSet<Collider2D>
                {
                    entity.GetComponent<Collider2D>()
                };
                newProc.GetComponent<Stormbringer_chain>().Initialize(this.damage, maxTargets, newTracker);
                newProc.SetActive(true);
            }
        }

        //Priority does not matter here.
        public Priority ReturnPriority() => Priority.First;

        public override void ToggleGear(bool state)
        {
            if (state)
                DynamicActionExecutor.Instance.AddDynamicAction(this);
            else
                DynamicActionExecutor.Instance.RemoveDynamicAction(this);
        }
    }

}
