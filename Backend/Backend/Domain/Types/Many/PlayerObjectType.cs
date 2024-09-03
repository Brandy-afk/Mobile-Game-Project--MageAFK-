using Backend.Domain.Modals;

namespace Backend.Domain.Types.Many
{
    public class PlayerObjectType : ObjectType<Player>
    {
        protected override void Configure(IObjectTypeDescriptor<Player> descriptor)
        {
            descriptor
             .Description("Represents a player in the game system");

            descriptor
                .Field(p => p.ID)
                .Description("The unique identifier of the player")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(p => p.Username)
                .Description("The username of the player")
                .Type<NonNullType<StringType>>();

            descriptor.Ignore(p => p.CreatedAt);
            descriptor.Ignore(p => p.UpdatedAt);
        }


    }
}
