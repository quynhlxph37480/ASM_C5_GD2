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
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet("get-all-product")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("get-product-by-id")]
        public async Task<ActionResult<Product>> GetProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("edit-product")]
        public async Task<IActionResult> PutProduct(Guid id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("create-product")]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("delete-product")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(Guid id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart(Guid id, int quantity, string usn)
        {

                //Lay ra tu danh sach CartDetails ung vs user do xem co sp nao trung id k
                var cartItem = _context.CartsDetails.FirstOrDefault(x => x.Username == usn && x.ProductId == id);
                //Neu san pham chua ton tai <=> cartitem co gia tri null => Tao ra 1 cart details vs username
                //la tai khoan dang dang nhap va id product la san pham duoc chon va so luong dc chon
                if (cartItem == null)
                {
                    CartDetails cartDetails = new CartDetails()
                    {
                        Id = Guid.NewGuid(),
                        ProductId = id,
                        Username = usn,
                        Quantity = quantity,
                        Status = 1
                    };

                    var product = _context.Products.FirstOrDefault(x => x.Id == id);

                    if (cartDetails.Quantity > product.Quantity)
                    {
                        return Ok();
                    }
                    else
                    {
                        _context.CartsDetails.Add(cartDetails);
                        await _context.SaveChangesAsync();
                        return Ok("Them vao cart thanh cong");
                    }

                }
                else
                {

                    var product = _context.Products.FirstOrDefault(x => x.Id == id);
                    cartItem.Quantity = cartItem.Quantity + quantity;

                    if (cartItem.Quantity > product.Quantity)
                    {
                        return Ok("Khong du so luong");
                    }
                    else
                    {
                        _context.CartsDetails.Update(cartItem);
                        await _context.SaveChangesAsync();
                        return Ok("Them thanh cong ");
                    }
                }
            
        }
    }
}
