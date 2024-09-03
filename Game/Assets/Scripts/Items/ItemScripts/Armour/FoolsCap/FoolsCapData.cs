
using MageAFK.AI;
using MageAFK.Combat;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Pooling;
using MageAFK.Spells;
using MageAFK.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.Items
{

  [CreateAssetMenu(fileName = "Stormbringer", menuName = "Items/ItemData/Equippable/FoolsCap")]
  public class FoolsCapData : ArmourData, IPrefabReturner
  {
    [BoxGroup("FoolsCap")] public float[] damage;
    [BoxGroup("FoolsCap")] public float[] chance;
    [BoxGroup("FoolsCap"), SerializeField] private GameObject prefab;
    public override string ReturnArmourInfo(ItemLevel level, string highlightHex)
    {
      int intLevel = (int)level;
      string format = $"When you fear enemies there is a <color=#{{2}}>{{1}}%</color> chance for them to explode, dealing <color=#{{2}}>{{0}}</color> damage.";
      return string.Format(format, damage[intLevel], chance[intLevel], highlightHex);
    }
    public GameObject ReturnPrefab() => prefab;
  }

  public class FoolsCap : Armour, ISpellCollisionEvent
  {

    public readonly float chance;
    public readonly float damage;

    public FoolsCap(FoolsCapData data, ItemLevel level) : base(data, level)
    {
      int integerLevel = (int)level;
      damage = data.damage[integerLevel];
      chance = data.chance[integerLevel];
    }

    public void HandleCollision(Spell spell, NPEntity entity, ref float damage, ref float mod, bool isCrit, bool isPierce, bool isEffect)
    {
      if (isEffect && spell.effect == StatusType.Fear && Utility.RollChance(chance))
      {
        var newObj = ServiceLocator.Get<ItemPooler>().Get(iD);
        newObj.transform.position = entity.transform.position;
        newObj.SetActive(true);
      }
    }

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