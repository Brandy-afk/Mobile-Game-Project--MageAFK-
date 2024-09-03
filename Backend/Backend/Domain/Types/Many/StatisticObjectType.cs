using Backend.Domain.Modals.PlayerStatistics;

namespace Backend.Domain.Types.Many
{
    public class StatisticObjectType : ObjectType<Statistic>
    {
        protected override void Configure(IObjectTypeDescriptor<Statistic> descriptor)
        {
            descriptor
                .Description("Represents a general statistic for a player");

            descriptor
                .Field(s => s.ID)
                .Description("The unique identifier of the statistic")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(s => s.PlayerID)
                .Description("The ID of the player associated with this statistic")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(s => s.TypeID)
                .Description("The ID of the statistic type")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(s => s.Value)
                .Description("The value of the statistic")
                .Type<NonNullType<FloatType>>();
        }
    }
}
