using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace App.Data.Entities
{
    //Data Annotation Validation cho phep chung ta validate du lieu ngay tu Models
    public class Account
    {
		// Data Anotation Validation cho phép chúng ta validate dữ liệu ngay từ Models
		[Required]
		[StringLength(450, MinimumLength = 10, ErrorMessage = "Độ dài Username phải từ 10 đến 450")]
		public string Username { get; set; }
		[Required]
		[StringLength(450, MinimumLength = 10, ErrorMessage = "Độ dài Username phải từ 10 đến 450")]
		public string Password { get; set; }

		[RegularExpression(@"^0[3-9][0-9]{8}$", ErrorMessage = "So dien thoai phai dung format va co 10 chu so")]
        public string PhoneNumber {  get; set; }

        [DisplayName("Địa chỉ")]
        public string? Address { get; set; }

        public virtual List<Bill>? Bills { get; set; }
        public virtual Cart? Cart { get; set; }

    }
}
