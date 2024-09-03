namespace Backend.Domain.Modals
{
    public class History
    {
        public int ID { get; set; }
        public int PlayerID { get; set; }
        public DateTime Date { get; set; }
        public int LocationID { get;  set; }
        public bool BestWave { get; set; }
        public float Damage { get; set; }
        public int Level { get; set; }
        public int Wave { get; set; }
        public string Metrics { get; set; }
        public List<int> Mob { get; set; }
        public List<int> Spell { get; set; }

    }
}
