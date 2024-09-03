namespace Backend.Domain.Modals.Milestones
{
    public class Milestone
    {
        public int ID { get; set; }
        public int PlayerID { get; set; }
        public int TypeID { get; set; }
        public float Value { get; set; }
        public int Rank { get; set; }
        public int RewardPoolSize { get; set; }
    }
}
