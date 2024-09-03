using Backend.Domain.Enums;
using Backend.Domain.Inputs.DTO;

namespace Backend.Domain.Inputs.Types
{
    public class AbstractTypeInputType : InputObjectType<AbstractTypeInput>
    {
        protected override void Configure(IInputObjectTypeDescriptor<AbstractTypeInput> descriptor)
        {
            descriptor
                .Description("Input type for creating or updating an abstract type (Tables that help better define what is in 'Many' tables)");

            descriptor
               .Field(f => f.ID)
               .Description("The ID of the type. Use 0 for new entries.")
               .Type<NonNullType<IntType>>();

            descriptor
                .Field(f => f.Name)
                .Description("The name of the type.")
                .Type<NonNullType<StringType>>();

            descriptor
                .Field(f => f.Description)
                .Description("A description of the type.")
                .Type<StringType>();

            descriptor
                .Field(f => f.Type)
                .Description("The specific table type for this entry.")
                .Type<NonNullType<EnumType<TableTypeEnum>>>();
        }
    }
}
