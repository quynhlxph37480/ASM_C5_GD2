namespace App.Data.Entities
{
    public class Bill
    {
		public string Id { get; set; }
		public string Username { get; set; }
		public int Status { get; set; }
		public DateTime CreateDate { get; set; }
		public virtual List<BillDetails>? BillDetails { get; set; }
		public virtual Account Account { get; set; }
	}
}
