namespace BaseLibrary.DTOs
{
    public class ProductDTO : BaseEntityDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; } = string.Empty;
        public int CategoryId { get; set; }
    }
}
