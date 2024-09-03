using Backend.Domain.Modals;

namespace Backend.Domain.Types.Many
{
    public class ElixirObjectType : ObjectType<Elixir>
    {
        protected override void Configure(IObjectTypeDescriptor<Elixir> descriptor)
        {
            descriptor
                .Description("Represents an elixir shop entry for a player");

            descriptor
                .Field(e => e.ID)
                .Description("The unique identifier of the elixir shop entry")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(e => e.PlayerID)
                .Description("The ID of the player associated with this elixir shop entry")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(e => e.Purchased)
                .Description("Indicates whether the elixir has been purchased")
                .Type<NonNullType<BooleanType>>();

            descriptor
                .Field(e => e.Cost)
                .Description("The cost of the elixir")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(e => e.Value)
                .Description("The value of the elixir")
                .Type<NonNullType<FloatType>>();
        }
    }
}
