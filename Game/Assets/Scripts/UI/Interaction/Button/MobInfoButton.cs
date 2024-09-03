using MageAFK.AI;

namespace MageAFK.UI
{
    public class MobInfoButton : TabButton
  {
    private Mob mob;

    public void SetMob(Mob mob)
    {
      this.mob = mob;
    }

    public Mob ReturnMob()
    {
      return mob;
    }

  }
}