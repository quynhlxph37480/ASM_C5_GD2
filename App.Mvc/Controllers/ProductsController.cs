using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Data.Data;
using App.Data.Entities;
using Newtonsoft.Json;
using System.Text;

namespace OnTap_NET104.Controllers.Admin
{
    public class ProductsController : Controller
    {
        private readonly ILogger<ProductsController> _logger;
        public HttpClient _client;

        public ProductsController(ILogger<ProductsController> logger)
        {
            _client = new HttpClient();
            _logger = logger;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            string requestUrl = "https://localhost:7215/api/Products/get-all-product";

            var response = _client.GetStringAsync(requestUrl).Result;
            
            if(response != null)
            {
                List<Product> _lst = JsonConvert.DeserializeObject<List<Product>>(response);
                return View(_lst);
            }
            return RedirectToAction("Index", "Home");
                    
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            string requestUrl = $"https://localhost:7215/api/Products/get-product-by-id?id={id}";

            var response = _client.GetStringAsync(requestUrl).Result;

            if (response != null)
            {
                Product product = JsonConvert.DeserializeObject<Product>(response);
                return View(product);
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

          [HttpPost]
           [ValidateAntiForgeryToken]
           public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,Quantity,Status")] Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string requestUrl = $"https://localhost:7215/api/Products/create-product";


                    string json = JsonConvert.SerializeObject(product);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");


                    var response = await _client.PostAsync(requestUrl, content);

                    if(response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "Products");
                    }
                    else
                    {
                        return View(product);
                    }


                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi rồi nè !");
                    return BadRequest(ex.Message);
                }
            }
            return View(product);
        }

        // GET: Products/Edit/5
        //public async Task<IActionResult> Edit(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await _context.Products.FindAsync(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(product);
        //}

        //// POST: Products/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Description,Price,Quantity,Status")] Product product)
        //{
        //    if (id != product.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(product);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ProductExists(product.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(product);
        //}

        // GET: Products/Delete/5
        //public async Task<IActionResult> Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await _context.Products
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(product);
        //}

        //// POST: Products/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(Guid id)
        //{
        //    var product = await _context.Products.FindAsync(id);
        //    if (product != null)
        //    {
        //        _context.Products.Remove(product);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool ProductExists(Guid id)
        //{
        //    return _context.Products.Any(e => e.Id == id);
        //}

        public async Task<IActionResult> AddToCart(Guid id, int quantity)
        {
            //kiem tra xem co dang nhap khong
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Accounts");//chuyen huong ve trang login
            }
            else
            {
                string usn = User.Identity.Name;
                string requestUrl = $"https://localhost:7215/api/Products/add-to-cart?id={id}&quantity={quantity}&usn={usn}";

                // Chuẩn bị dữ liệu JSON cần gửi
                var data = new
                {
                    id = id, 
                    quantity = quantity,      
                    usn = usn   
                };

                // Chuyển đổi dữ liệu sang định dạng JSON
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = _client.PostAsync(requestUrl, content).Result;

                if(response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Carts");
                }
                else
                {
                    TempData["ErrorQuantityMessage"] = "Quantity in stock is not enough !";
                    return RedirectToAction("Index", "Products");
                }


                ////Lay ra tu danh sach CartDetails ung vs user do xem co sp nao trung id k
                //var cartItem = _context.CartsDetails.FirstOrDefault(x => x.Username == check && x.ProductId == id);
                ////Neu san pham chua ton tai <=> cartitem co gia tri null => Tao ra 1 cart details vs username
                ////la tai khoan dang dang nhap va id product la san pham duoc chon va so luong dc chon
                //if (cartItem == null)
                //{
                //    CartDetails cartDetails = new CartDetails()
                //    {
                //        Id = Guid.NewGuid(),
                //        ProductId = id,
                //        Username = check,
                //        Quantity = quantity,
                //        Status = 1
                //    };

                //    var product = _context.Products.FirstOrDefault(x => x.Id == id);

                //    if (cartDetails.Quantity > product.Quantity)
                //    {
                //        TempData["ErrorQuantityMessage"] = "Quantity in stock is not enough !";
                //        return RedirectToAction("Index", "Products");
                //    }
                //    else
                //    {
                //        _context.CartsDetails.Add(cartDetails);
                //        await _context.SaveChangesAsync();
                //        return RedirectToAction("Index", "Carts");
                //    }

                //}
                //else
                //{

                //    var product = _context.Products.FirstOrDefault(x => x.Id == id);
                //    cartItem.Quantity = cartItem.Quantity + quantity; //chua check trong kho

                //    if (cartItem.Quantity > product.Quantity)
                //    {
                //        TempData["ErrorQuantityMessage"] = "Quantity in stock is not enough !";
                //        return RedirectToAction("Index", "Products");
                //    }
                //    else
                //    {
                //        _context.CartsDetails.Update(cartItem);
                //        await _context.SaveChangesAsync();
                //        return RedirectToAction("Index", "Carts");
                //    }
                //}
            }
        }
    }
}
