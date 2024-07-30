namespace minimalAPIMongo.ViewModels
{
    public class CreateOrderViewModel
    {
        public DateTime Date { get; set; }
        public string? Status { get; set; }
        public string? ClientId { get; set; }
        public List<ProductReference>? Products { get; set; }

        public class ProductReference
        {
            public string? ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
