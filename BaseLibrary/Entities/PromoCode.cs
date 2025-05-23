namespace BaseLibrary.Entities
{
    public class PromoCode : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool isActive { get; set; }
        public DateTime ExpireDate { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();

    }
}