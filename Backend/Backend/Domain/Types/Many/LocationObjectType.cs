using Location = Backend.Domain.Modals.Location.Location;

namespace Backend.Domain.Types.Many
{
    public class LocationObjectType : ObjectType<Location>
    {
        protected override void Configure(IObjectTypeDescriptor<Location> descriptor)
        {
            descriptor
                .Description("Represents a location associated with a player");

            descriptor
                .Field(l => l.ID)
                .Description("The unique identifier of the location entry")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(l => l.PlayerID)
                .Description("The ID of the player associated with this location")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(l => l.TypeID)
                .Description("The ID of the location type")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(l => l.BestWave)
                .Description("The highest wave achieved by the player at this location")
                .Type<NonNullType<IntType>>();
        }
    }
}
