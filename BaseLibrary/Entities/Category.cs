using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BaseLibrary.Entities
{
    public class Category : BaseEntity
    {
        [Required(ErrorMessage = "Название категории обязательно к заполнению")]
        [MaxLength(100, ErrorMessage = "Название не должно превышать 100 ")]
        public string Name { get; set; } = "";
        //public int ChildCategoryId { get; set; }

        [JsonIgnore]
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
