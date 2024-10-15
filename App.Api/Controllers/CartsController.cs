using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.Data.Data;
using App.Data.Entities;

namespace App.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Carts/5
        [HttpGet("get-cartdetails")]
        public IActionResult GetCart(string id)
        {
            return Ok(_context.CartsDetails.Include(x => x.Product).Where(x => x.Username == id).ToList());
        }

        // PUT: api/Carts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCart(string id, Cart cart)
        {
            if (id != cart.Username)
            {
                return BadRequest();
            }

            _context.Entry(cart).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Carts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("create-cart")]
        public async Task<ActionResult<Cart>> PostCart(Cart cart)
        {
            _context.Carts.Add(cart);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CartExists(cart.Username))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCart", new { id = cart.Username }, cart);
        }

        // DELETE: api/Carts/5
        [HttpDelete("delete-cartdetails")]
        public async Task<IActionResult> DeleteCartDetails(Guid id)
        {

            CartDetails cartdt = _context.CartsDetails.FirstOrDefault(x => x.Id.Equals(id));
            if (cartdt != null)
            {
                _context.CartsDetails.Remove(cartdt);
                _context.SaveChanges();
            }


            return NoContent();
        }

        private bool CartExists(string id)
        {
            return _context.Carts.Any(e => e.Username == id);
        }

        Random _random = new Random();

        [HttpGet("random-id")]
        public int RandomId()
        {
            Bill bill;
            int randomNumber;
            do
            {
                randomNumber = _random.Next(100000, 1000000); // 1000000 là exclusive, nên nó chỉ sinh từ 100000 đến 999999

                bill = _context.Bills.FirstOrDefault(x => x.Id == $"B{randomNumber}");
            } while (bill != null);

            return randomNumber;
        }

        [HttpPost("thanh-toan")]
        public IActionResult ThanhToan(string usn)
        {
            bool checkTT = true;
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    string Id = "B" + RandomId();

                    Bill bill = new Bill()
                    {
                        Id = Id,
                        Username = usn,
                        Status = 0,
                        CreateDate = DateTime.Now,
                    };

                    _context.Bills.Add(bill); //tạo bill

                    foreach (var item in _context.CartsDetails.Include(x => x.Product).Where(x => x.Username == usn).ToList())
                    {
                        if (item.Product.Quantity < item.Quantity) //check xem đủ số lượng không
                        {
                            checkTT = false; //không đủ false và thoát vòng lặp
                            break;
                        }

                        item.Product.Quantity -= item.Quantity; //đủ trừ số lượng trong kho

                        BillDetails bdetail = new BillDetails() //tạo billdetails 
                        {
                            Id = Guid.NewGuid(),
                            BillId = bill.Id,
                            ProductId = item.ProductId,
                            ProductPrice = item.Product.Price,
                            Quantity = item.Quantity,
                            Status = 1
                        };

                        _context.BillDetails.Add(bdetail);
                        _context.Products.Update(item.Product);  //Update số lượng (chưa lưu)
                    }

                    if (checkTT) //check xem quá trình thanh toán có đoạn nào không thành công không 
                    {
                        _context.SaveChanges();
                        transaction.Commit(); //nếu true hết thì lưu dữ liệu (commit xác nhận lưu tất cả các thao tác CRUD thanh toán)


                        // Xóa các sản phẩm trong giỏ hàng sau khi thanh toán
                        var cartItems = _context.CartsDetails.Where(x => x.Username == usn).ToList();
                        _context.CartsDetails.RemoveRange(cartItems);
                        _context.SaveChanges();

                        return Ok("TT thanh cong");
                    }
                    else
                    {
                        transaction.Rollback(); //nếu có 1 đoạn bị false quay lại hết quá trình thanh toán như ban đầu 
                        return BadRequest("TT that bai");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); //nếu có đoạn nào đó bị lỗi (Exception) thì cũng quay lại như ban đầu
                    return BadRequest("TT that bai, loi:  \n" + ex.Message);
                }
            }
        }
    }
}
