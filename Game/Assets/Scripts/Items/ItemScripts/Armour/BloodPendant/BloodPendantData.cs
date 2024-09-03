
using MageAFK.AI;
using MageAFK.Combat;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Pooling;
using MageAFK.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.Items
{
    [CreateAssetMenu(fileName = "New Blood Pendant", menuName = "Items/ItemData/Equippable/BloodPendantData")]
    public class BloodPendantData : ArmourData, IPrefabReturner
    {
        [BoxGroup("BloodPendant")]
        public int[] maxSpawns = new int[4];
        [BoxGroup("BloodPendant")]
        public float[] damage = new float[4];

        [BoxGroup("BloodPendant"), SerializeField]
        private GameObject prefab;

        public override string ReturnArmourInfo(ItemLevel level, string highlightHex)
        {
            int intLevel = (int)level;
            string format = $"Taking damage will spawn a blood mine randomly. It will roam the map dealing <color=#{{2}}>{{0}}</color> damage on impact. A maximum of <color=#{{2}}>{{1}}</color> can be spawned";
            return string.Format(format, damage[intLevel], maxSpawns[intLevel], highlightHex);
        }

        public GameObject ReturnPrefab() => prefab;
    }

    public class BloodPendant : Armour, IPlayerCollisionEvent
    {
        public readonly float damage;
        public readonly int maxSpawns;
        private int spawnCount = 0;

        public BloodPendant(BloodPendantData data, ItemLevel level) : base(data, level)
        {
            int integerLevel = (int)level;
            damage = data.damage[integerLevel];
            maxSpawns = data.maxSpawns[integerLevel];
        }

        public void HandleCollision(NPEntity entity, ref float damage, ref float mod)
        {
            if (spawnCount < maxSpawns)
            {
                spawnCount++;
                var newObj = ServiceLocator.Get<ItemPooler>().Get(iD);
                newObj.transform.position = Utility.GetRandomMapPosition();
                newObj.SetActive(true);
            }
        }

        public void ReduceSpawnedObjects() => spawnCount--;

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
