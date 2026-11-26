namespace GameStore.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public decimal PurchasePrice { get; set; }
        public decimal RetailPrice { get; set; }
    }



}
