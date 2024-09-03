using Backend.Domain.Modals.Milestones;

namespace Backend.Domain.Types.Many
{
    public class MilestoneObjectType : ObjectType<Milestone>
    {
        protected override void Configure(IObjectTypeDescriptor<Milestone> descriptor)
        {
            descriptor
                .Description("Represents a milestone achievement for a player");

            descriptor
                .Field(m => m.ID)
                .Description("The unique identifier of the milestone")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(m => m.PlayerID)
                .Description("The ID of the player who achieved this milestone")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(m => m.TypeID)
                .Description("The ID of the milestone type")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(m => m.Value)
                .Description("The value associated with this milestone")
                .Type<NonNullType<FloatType>>();

            descriptor
                .Field(m => m.Rank)
                .Description("The rank or level of this milestone achievement")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(m => m.RewardPoolSize)
                .Description("The size of the reward pool for this milestone")
                .Type<NonNullType<IntType>>();
        }
    }
}
