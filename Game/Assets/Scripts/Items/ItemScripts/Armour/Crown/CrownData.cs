using System;
using MageAFK.AI;
using MageAFK.Combat;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Pooling;
using UnityEngine;

namespace MageAFK.Items
{
    [CreateAssetMenu(fileName = "Crown", menuName = "Items/ItemData/Equippable/CrownData")]
    public class CrownData : ArmourData, IPrefabReturner
    {
        [SerializeField, Header("Regular form please ex: 20 -> 20%")] public float[] smitePercentage;
        [SerializeField] public float timeBetweenEffects = 5f;
        [SerializeField] private GameObject prefab;

        public GameObject ReturnPrefab() => prefab;
        public override string ReturnArmourInfo(ItemLevel level, string highlightHex)
        {
            int intLevel = (int)level;
            string format = $"Every <color=#{{2}}>{{0}}</color> seconds, all enemies will be smited for <color=#{{2}}>{{1}}%</color> of their max health";
            return string.Format(format, timeBetweenEffects, smitePercentage[intLevel], highlightHex);
        }
    }

    public class Crown : Armour, IWaveUpdateAction
    {
        public readonly float damage;
        public readonly float timeBetweenEffects;
        public Crown(CrownData data, ItemLevel level) : base(data, level)
        {
            int intLevel = (int)level;
            damage = data.smitePercentage[intLevel];
            timeBetweenEffects = data.timeBetweenEffects;
        }


        public override void ToggleGear(bool state)
        {
            if (state)
                DynamicActionExecutor.Instance.AddDynamicAction(this);
            else
                DynamicActionExecutor.Instance.RemoveDynamicAction(this);
        }

        public float ReturnTimeBetweenUpdates() => timeBetweenEffects;

        public void UpdateAction()
        {
            var itemObj = ServiceLocator.Get<ItemPooler>().Get(iD);
            itemObj.transform.position = PlayerController.Positions.Pivot;
            itemObj.SetActive(true);
        }

        public void ApplyEffect()
        {
            try
            {
                foreach (var entity in ServiceLocator.Get<EntityDataManager>().ReturnActiveNPEntities())
                    HolyEffect.Apply(entity, damage, (int)iD, OrginType.Item);
            }
            catch (InvalidOperationException)
            {
                return;
            }
        }
    }
}
