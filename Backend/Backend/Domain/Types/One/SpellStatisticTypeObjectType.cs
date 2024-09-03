using Backend.Domain.Modals;
using Backend.Domain.Modals.Currency;
using Backend.Domain.Modals.Spells;

namespace Backend.Domain.Types.One
{
    public class SpellStatisticTypeObjectType : AbstractTypeObjectType<SpellStatisticType>
    {
        protected override void Configure(IObjectTypeDescriptor<SpellStatisticType> descriptor)
        {
            base.Configure(descriptor);
        }
    }
}
