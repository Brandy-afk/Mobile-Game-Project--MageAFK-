using Backend.Domain.Modals;
using Backend.Domain.Modals.Currency;
using Backend.Domain.Modals.Location;
using Backend.Domain.Modals.Milestones;

namespace Backend.Domain.Types.One
{
    public class MilestoneTypeObjectType : AbstractTypeObjectType<MilestoneType>
    {
        protected override void Configure(IObjectTypeDescriptor<MilestoneType> descriptor)
        {
            base.Configure(descriptor);
        }
    }
}
