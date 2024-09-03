using Backend.Domain.Modals;
using Backend.Domain.Modals.Currency;
using Backend.Domain.Modals.Recipes;
using Backend.Domain.Modals.Skills;

namespace Backend.Domain.Types.One
{
    public class SkillTypeObjectType : AbstractTypeObjectType<SkillType>
    {
        protected override void Configure(IObjectTypeDescriptor<SkillType> descriptor)
        {
            base.Configure(descriptor);
        }
    }
}
