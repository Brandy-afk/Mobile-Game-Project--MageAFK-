using Backend.Domain.Modals;
using Backend.Domain.Modals.Currency;
using Backend.Domain.Modals.PlayerStatistics;
using Backend.Domain.Modals.Spells;

namespace Backend.Domain.Types.One
{
    public class StatisticTypeObjectType : AbstractTypeObjectType<StatisticType>
    {
        protected override void Configure(IObjectTypeDescriptor<StatisticType> descriptor)
        {
            base.Configure(descriptor);
        }
    }
}
