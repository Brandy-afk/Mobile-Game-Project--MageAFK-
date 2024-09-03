using Backend.Domain.Modals.Skills;

namespace Backend.Domain.Types.Many
{
    public class SkillObjectType : ObjectType<Skill>
    {
        protected override void Configure(IObjectTypeDescriptor<Skill> descriptor)
        {
            descriptor
                .Description("Represents a skill for a player");

            descriptor
                .Field(s => s.ID)
                .Description("The unique identifier of the skill")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(s => s.PlayerID)
                .Description("The ID of the player who owns this skill")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(s => s.TypeID)
                .Description("The ID of the skill type")
                .Type<NonNullType<IntType>>();


            descriptor
                .Field(s => s.Rank)
                .Description("The current rank of the skill")
                .Type<NonNullType<IntType>>();
        }
    }

}
