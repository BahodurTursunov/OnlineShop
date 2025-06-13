using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BaseLibrary.Entities
{
    public class Category : BaseEntity
    {
        [Required(ErrorMessage = "Название категории обязательно к заполнению")]
        public string Name { get; set; } = "";
        //public int ChildCategoryId { get; set; }

        [JsonIgnore]
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
