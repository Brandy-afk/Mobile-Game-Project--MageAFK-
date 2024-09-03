using Backend.Domain.Modals.Currency;

namespace Backend.Domain.Types.Many
{
    public class CurrencyObjectType : ObjectType<Currency>
    {
        protected override void Configure(IObjectTypeDescriptor<Currency> descriptor)
        {
            descriptor
                .Description("Represents a currency entry for a player");

            descriptor
                .Field(c => c.ID)
                .Description("The unique identifier of the currency entry")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(c => c.PlayerID)
                .Description("The ID of the player who owns this currency")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(c => c.TypeID)
                .Description("The ID of the currency type")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(c => c.Value)
                .Description("The amount of currency")
                .Type<NonNullType<IntType>>();
        }
    }
}
