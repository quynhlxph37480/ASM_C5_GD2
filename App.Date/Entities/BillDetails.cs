namespace App.Data.Entities
{
    public class BillDetails //mỗi billdetails chứa thông tin của một sản phẩm trong bill
    {
		public Guid Id { get; set; }
		public string BillId { get; set; }
		public Guid ProductId { get; set; }
		public decimal ProductPrice { get; set; } // Giá tại thời điểm mua
		public int Quantity { get; set; }
		public int Status { get; set; }
		public virtual Bill Bill { get; set; }
		public virtual Product Product { get; set; }
	}
}
