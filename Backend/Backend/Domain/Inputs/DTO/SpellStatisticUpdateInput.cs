namespace Backend.Domain.Inputs.DTO
{
    public class SpellStatisticUpdateInput
    {
        public float Damage { get; set; }
        public int Kills { get; set; }
        public int Upgraded { get; set; }
        public int Cast { get; set; }
    }
}
