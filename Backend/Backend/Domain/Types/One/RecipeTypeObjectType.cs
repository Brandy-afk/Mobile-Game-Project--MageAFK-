using Backend.Domain.Modals;
using Backend.Domain.Modals.Currency;
using Backend.Domain.Modals.Milestones;
using Backend.Domain.Modals.Recipes;

namespace Backend.Domain.Types.One
{
    public class RecipeTypeObjectType : AbstractTypeObjectType<RecipeType>
    {
        protected override void Configure(IObjectTypeDescriptor<RecipeType> descriptor)
        {
            base.Configure(descriptor);
        }
    }
}
