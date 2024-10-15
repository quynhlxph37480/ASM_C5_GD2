using App.Data.Data;
using App.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.Xml;

namespace App.Mvc.Controllers
{
    public class CartsController : Controller
    {
        HttpClient _client;

        public CartsController()
        {
            _client = new HttpClient();
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var username = User.Identity.Name;
                if (username != null)
                {

                    string requestUrl = $"https://localhost:7215/api/Carts/get-cartdetails?id={username}";

                    var response = _client.GetStringAsync(requestUrl).Result;

                    var allCartItem = JsonConvert.DeserializeObject<List<CartDetails>>(response);

                    return View(allCartItem);

                }
                else
                {
                    return View();
                }
            
            }
            else
            {
                return RedirectToAction("Login", "Accounts");
            }

        }

        public IActionResult Delete(Guid id) 
        {
            string requestUrl = $"https://localhost:7215/api/Carts/delete-cartdetails?id={id}";

            var response = _client.DeleteAsync(requestUrl).Result;

            return RedirectToAction("Index");
        }

        public IActionResult ThanhToan()
        {
            string usn = User.Identity.Name;
            string requestUrl = $"https://localhost:7215/api/Carts/thanh-toan?usn={usn}";

            var response = _client.PostAsync(requestUrl, null).Result;

            if(response.IsSuccessStatusCode)
            {
                TempData["thanhcong"] = "Thanh toán thành công";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["thatbai"] = "Thanh toán thất bại";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
