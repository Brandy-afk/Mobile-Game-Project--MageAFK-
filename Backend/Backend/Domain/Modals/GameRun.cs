namespace Backend.Domain.Modals
{
    public class GameRun
    {
        public int GameRunID { get; set; }
        public int PlayerID { get; set; }
        public string GameState { get; set; } // This will store the JSON
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
