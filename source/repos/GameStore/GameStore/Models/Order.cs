namespace GameStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public bool Shipped { get; set; }

        public IEnumerable<OrderLine> Lines { get; set; } = new List<OrderLine>();
    }
}
