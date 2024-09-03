using Backend.Domain.Modals;

namespace Backend.Domain.Types.Many
{
    public class GameRunObjectType : ObjectType<GameRun>
    {
        protected override void Configure(IObjectTypeDescriptor<GameRun> descriptor)
        {
            descriptor
                .Description("Represents a players session save");

            descriptor
                .Field(g => g.GameRunID)
                .Description("The unique identifier of the game run")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(g => g.PlayerID)
                .Description("The ID of the player associated with this game run")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(g => g.GameState)
                .Description("The JSON representation of the game state")
                .Type<NonNullType<StringType>>();

            descriptor
                .Field(g => g.CreatedAt)
                .Description("The date and time when the game run was created")
                .Type<NonNullType<DateTimeType>>();

            descriptor
                .Field(g => g.CompletedAt)
                .Description("The date and time when the game run was completed, if applicable")
                .Type<DateTimeType>();
        }
    }
}
