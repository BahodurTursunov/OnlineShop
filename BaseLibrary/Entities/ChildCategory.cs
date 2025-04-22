using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Entities
{
    public class ChildCategory : BaseEntity
    {
        [MaxLength(100)]
        public string Name { get; set; } = "";
        public int CategoryId { get; set; }
        public required Category Category { get; set; }
    }
}
