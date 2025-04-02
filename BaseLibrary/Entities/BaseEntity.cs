using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
