namespace Backend.Domain.Modals
{
    public class Leaderboard
    {
        public int ID { get; set; }

        public int PlayerID { get; set; }

        public int LocationID { get; set; }

        public int Wave { get; set; }

        public DateTime Created { get; set; }

    }
}
