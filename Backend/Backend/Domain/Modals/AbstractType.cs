using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;

namespace Backend.Domain.Modals
{
    public class AbstractType
    {
        [Key]
        public int TypeID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
