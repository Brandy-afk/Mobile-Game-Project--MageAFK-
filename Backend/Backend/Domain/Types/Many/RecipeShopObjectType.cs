using Backend.Domain.Modals;

namespace Backend.Domain.Types.Many
{
    public class RecipeShopObjectType : ObjectType<RecipeShop>
    {
        protected override void Configure(IObjectTypeDescriptor<RecipeShop> descriptor)
        {
            descriptor
                .Description("Represents a recipe shop for a player");

            descriptor
                .Field(rs => rs.ID)
                .Description("The unique identifier of the recipe shop")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(rs => rs.PlayerID)
                .Description("The ID of the player associated with this recipe shop")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(rs => rs.RecipeIDs)
                .Description("List of recipe IDs available in the shop")
                .Type<NonNullType<ListType<NonNullType<IntType>>>>();
        }
    }
}
