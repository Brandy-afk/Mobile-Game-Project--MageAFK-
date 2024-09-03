using Backend.Domain.Enums;

namespace Backend.Domain.Inputs.DTO
{
    public class AbstractTypeInput
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TableTypeEnum Type { get; set; }

    }



}
