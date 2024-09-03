namespace Backend.Domain.Modals
{
    public class RecipeShop
    {
        public int ID { get; set; }
        public int PlayerID { get; set; }
        public List<int> RecipeIDs { get; set; }
    }
}
