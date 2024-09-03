using Backend.Domain.Modals;
using Backend.Domain.Modals.Currency;

namespace Backend.Domain.Types.One
{
    public class CurrencyTypeObjectType : AbstractTypeObjectType<CurrencyType>
    {
        protected override void Configure(IObjectTypeDescriptor<CurrencyType> descriptor)
        {
            base.Configure(descriptor);
        }
    }
}
