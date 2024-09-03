
using System.Collections.Generic;
using MageAFK.AI;
using MageAFK.Stats;
using MageAFK.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "GlacialWall", menuName = "Spells/GlacialWall")]
  public class GlacialWall : Spell
  {
    [BoxGroup("GlacialDecoy")]
    [SerializeField] private ColorPair rangeShader;
    private static Dictionary<Enemy, int> count = new();

    public override void Activate() => SpellSpawn(iD, Utility.GetRandomMapPosition()).GetComponent<GlacialWallController>();

    public override void OnWaveOver() => count.Clear();

    public static void OnEnter(Enemy enemy, Spell spell)
    {
      count.TryAdd(enemy, 0);
      count[enemy]++;

      enemy.StatusHandler.CreateEffect(Combat.OrginType.Spell
                                                , StatusType.Slow
                                                , spell.ReturnStatValue(Stat.Slow)
                                                , -5
                                                , (int)spell.iD);
    }

    public static void OnLeave(Enemy enemy, Spell spell)
    {
      if (count.ContainsKey(enemy)) count[enemy]--;
      else enemy.StatusHandler.TryRemoveEffect(StatusType.Slow, Combat.OrginType.Spell, (int)spell.iD);

      if (count[enemy] <= 0)
      {
        enemy.StatusHandler.TryRemoveEffect(StatusType.Slow, Combat.OrginType.Spell, (int)spell.iD);
        count.Remove(enemy);
      }
    }




  }
}
