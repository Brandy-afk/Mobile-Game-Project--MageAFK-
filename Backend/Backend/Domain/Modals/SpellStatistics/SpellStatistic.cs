using Backend.Domain.Modals.Skills;

namespace Backend.Domain.Modals.Spells
{
    public class SpellStatistic
    {
        public int ID { get; set; }
        public int PlayerID { get; set; }
        public int TypeID { get; set; }
        public float Damage { get; set; }
        public int Kills { get; set; }
        public int Upgraded { get; set; }
        public int Cast { get; set; }
    }
}
