namespace Backend.Domain.Modals.PlayerStatistics
{
    public class Statistic
    {
        public int ID { get; set; }
        public int PlayerID { get; set; }
        public int TypeID { get; set; }
        public float Value { get; set; }
    }
}
