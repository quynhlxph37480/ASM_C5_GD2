namespace App.Data.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
        public virtual List<BillDetails>? BillDetails { get; set; }
        public virtual List<CartDetails>? CartDetails { get; set; }

	}
}
