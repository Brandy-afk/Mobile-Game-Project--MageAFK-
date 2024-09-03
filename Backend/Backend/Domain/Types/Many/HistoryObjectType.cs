using Backend.Domain.Modals;

namespace Backend.Domain.Types.Many
{
    public class HistoryObjectType : ObjectType<History>
    {
        protected override void Configure(IObjectTypeDescriptor<History> descriptor)
        {
            descriptor
                .Description("Represents a historical record of a player's game session");

            descriptor
                .Field(h => h.ID)
                .Description("The unique identifier of the history entry")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(h => h.PlayerID)
                .Description("The ID of the player associated with this history entry")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(h => h.Date)
                .Description("The date and time of the recorded session")
                .Type<NonNullType<DateTimeType>>();

            descriptor
                .Field(h => h.LocationID)
                .Description("The location ID where the session took place")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(h => h.BestWave)
                .Description("Indicates if this session achieved the best wave for the player")
                .Type<NonNullType<BooleanType>>();

            descriptor
                .Field(h => h.Damage)
                .Description("The total damage dealt during the session")
                .Type<NonNullType<FloatType>>();

            descriptor
                .Field(h => h.Level)
                .Description("The player's level during this session")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(h => h.Wave)
                .Description("Information about the wave reached")
                .Type<NonNullType<StringType>>();

            descriptor
                .Field(h => h.Metrics)
                .Description("Additional metrics data for the session")
                .Type<NonNullType<StringType>>();

            descriptor
                .Field(h => h.Mob)
                .Description("List of mob-related integer values")
                .Type<NonNullType<ListType<NonNullType<IntType>>>>();

            descriptor
                .Field(h => h.Spell)
                .Description("List of spell-related integer values")
                .Type<NonNullType<ListType<NonNullType<IntType>>>>();
        }
    }
}
