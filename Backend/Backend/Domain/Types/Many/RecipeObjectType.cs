using Backend.Domain.Modals.Recipes;

namespace Backend.Domain.Types.Many
{
    public class RecipeObjectType : ObjectType<Recipe>
    {
        protected override void Configure(IObjectTypeDescriptor<Recipe> descriptor)
        {
            descriptor
                .Description("Represents a recipe for a player");

            descriptor
                .Field(r => r.ID)
                .Description("The unique identifier of the recipe")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(r => r.PlayerID)
                .Description("The ID of the player who owns this recipe")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(r => r.TypeID)
                .Description("The ID of the recipe type")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(r => r.Unlocked)
                .Description("Indicates whether the recipe is unlocked for the player")
                .Type<NonNullType<BooleanType>>();
        }
    }
}
