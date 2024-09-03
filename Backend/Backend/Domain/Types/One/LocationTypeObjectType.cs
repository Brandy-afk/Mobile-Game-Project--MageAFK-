
using Backend.Domain.Modals.Location;

namespace Backend.Domain.Types.One
{
    public class LocationTypeObjectType : AbstractTypeObjectType<LocationType>
    {
        protected override void Configure(IObjectTypeDescriptor<LocationType> descriptor)
        {
            base.Configure(descriptor);
        }
    }
}
