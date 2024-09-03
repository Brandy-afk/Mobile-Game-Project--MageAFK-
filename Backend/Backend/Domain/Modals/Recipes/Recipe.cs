namespace Backend.Domain.Modals.Recipes
{
    public class Recipe
    {
        public int ID { get; set; }
        public int PlayerID { get; set; }
        public int TypeID { get; set; }
        public bool Unlocked { get; set; }
    }
}
