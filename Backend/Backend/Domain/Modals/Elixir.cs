namespace Backend.Domain.Modals
{
    public class Elixir
    {
        public int ID { get; set; }
        public int ElixirID { get; set; }
        public int PlayerID { get; set; }
        public bool Purchased { get; set; }
        public int Cost { get; set; }
        public float Value { get; set; }
    }
}
