using Backend.Domain.Inputs.DTO;
using Backend.Domain.Modals;

namespace Backend.Domain.Types.One
{
    public abstract class AbstractTypeObjectType<T> : ObjectType<T> where T : AbstractType
    {
        protected override void Configure(IObjectTypeDescriptor<T> descriptor)
        {
            descriptor
                .Description("Base type for various type entities in the system.");

            descriptor
                .Field(t => t.TypeID)
                .Description("The unique identifier of the type.")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(t => t.Name)
                .Description("The name of the type.")
                .Type<NonNullType<StringType>>();

            descriptor
                .Field(t => t.Description)
                .Description("A description of the type.")
                .Type<StringType>()
                 .Resolve(context =>
                 {
                     var description = context.Parent<T>().Description;
                     return string.IsNullOrEmpty(description) ? "No description available" : description;
                 });
        }
    }

}
