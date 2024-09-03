using Backend.Domain.Modals.Spells;

namespace Backend.Domain.Types.Many
{
    public class SpellStatisticObjectType : ObjectType<SpellStatistic>
    {
        protected override void Configure(IObjectTypeDescriptor<SpellStatistic> descriptor)
        {
            descriptor
                .Description("Represents statistics for a player's spell");

            descriptor
                .Field(s => s.ID)
                .Description("The unique identifier of the spell statistic")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(s => s.PlayerID)
                .Description("The ID of the player associated with these spell statistics")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(s => s.TypeID)
                .Description("The ID of the spell type")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(s => s.Damage)
                .Description("The total damage dealt by this spell")
                .Type<NonNullType<FloatType>>();

            descriptor
                .Field(s => s.Kills)
                .Description("The number of kills achieved with this spell")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(s => s.Upgraded)
                .Description("The number of times this spell has been upgraded")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(s => s.Cast)
                .Description("The number of times this spell has been cast")
                .Type<NonNullType<IntType>>();
        }
    }
}
