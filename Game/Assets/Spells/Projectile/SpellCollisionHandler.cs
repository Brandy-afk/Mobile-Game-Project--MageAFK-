using UnityEngine;
using MageAFK.Player;
using MageAFK.Spells;
using MageAFK.Stats;
using MageAFK.AI;
using System.Collections.Generic;
using MageAFK.Tools;
using MageAFK.Management;
using MageAFK.UI;

namespace MageAFK.Combat
{
  public static class SpellCollisionHandler
  {


    private static Stat[] rollStats = { Stat.CritChance, Stat.PierceChance, Stat.StatusChance };

    private static Dictionary<SpellBook, Stat> bookStats = new()
    {
      {SpellBook.Soul, Stat.SoulDamage},
      {SpellBook.Chaos, Stat.ChaosDamage},
      {SpellBook.Life, Stat.LifeDamage},
      {SpellBook.Demonic, Stat.DemonicDamage}
    };

    public static CollisionInformation ReturnCollisionInformation(Spell spell, NPEntity enemy, bool forceCrit, bool forceStatus, bool forcePierce, float baseDamage)
    {
      List<bool> rolls = new();
      foreach (Stat stat in rollStats)
        rolls.Add(RollChance(stat, spell));

      rolls[0] = forceCrit ? true : rolls[0];
      rolls[1] = forcePierce ? true : rolls[1];
      rolls[2] = forceStatus ? true : rolls[2];

      var damage = ReturnDamage(spell, enemy, rolls[1], rolls[0], rolls[2], baseDamage);

      return new CollisionInformation(damage, forcePierce ? true : rolls[1], forceCrit ? true : rolls[0], forceStatus ? true : rolls[2]);
    }


    private static float ReturnDamage(Spell spell, NPEntity entity, bool isPierce, bool isCrit, bool isEffect, float baseDamage)
    {
      float damage = baseDamage == .01f ? spell.ReturnStatValue(Stat.Damage, false) : baseDamage;
      float modification = 0f;

      #region Basic / Intial Modification

      var playerStatHandler = ServiceLocator.Get<PlayerStatHandler>();
      if (spell.book != SpellBook.All)
      {
        modification += playerStatHandler.ReturnModification(bookStats[spell.book]);
      }
      modification += playerStatHandler.ReturnModification(Stat.Damage);

      #endregion

      #region Status Effects

      modification += PoisonEffect.ReturnBest(entity.StatusHandler.ReturnEffects(StatusType.Poison));

      #endregion

      #region Spells / Skills
      //Effect skills
      foreach (ISpellCollisionEvent collisionEvent in
      DynamicActionExecutor.Instance.spellCollisions)
        collisionEvent.HandleCollision(spell, entity, ref damage, ref modification, isCrit, isPierce, isEffect);

      #endregion

      damage += damage * modification;

      #region Basic Procs
      //Crit
      if (isCrit)
        damage += damage * spell.ReturnStatValue(Stat.CritDamage) / 100f;

      //If the ability did not pierce, reduce damage based on enemies armour.
      if (!isPierce)
        damage = Mathf.Max(0, damage - entity.runtimeStats[Stat.Armour]);

      #endregion

      return damage;
    }


    private static bool RollChance(Stat stat, Spell spell)
    {
      if (spell == null ||
      (stat == Stat.StatusChance &&
      spell.effect == StatusType.None))
        return false;

      return Utility.RollChance(spell.ReturnStatValue(stat));
    }


  }
  public struct CollisionInformation
  {
    public float damage;
    public bool isPierce;
    public bool isCrit;
    public bool isStatusProc;

    public TextInfoType textType;

    public CollisionInformation(float damage, bool isPierce, bool isCrit, bool isElementalProc)
    {
      this.damage = damage;
      this.isCrit = isCrit;
      this.isPierce = isPierce;
      isStatusProc = isElementalProc;

      if (isCrit)
        textType = TextInfoType.Crit;
      else if (isPierce)
        textType = TextInfoType.Pierce;
      else if (isStatusProc)
        textType = TextInfoType.Status;
      else
        textType = TextInfoType.Default;


    }
  }

}
