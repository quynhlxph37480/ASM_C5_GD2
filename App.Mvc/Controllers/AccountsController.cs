using App.Data.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using Newtonsoft.Json;

namespace App.Mvc.Controllers
{
    public class AccountsController : Controller
    {
        private readonly HttpClient _client;

        public AccountsController()
        {
            _client = new HttpClient();
        }
        public async Task<IActionResult> Login(string username, string password) //Dang nhap
        {
            if (username == null && password == null)
            {
                return View();
            }
            else
            {
                string requestUrl = $"https://localhost:7215/Auth/login?usn={username}&pass={password}";

                var response = await _client.PostAsJsonAsync(requestUrl, new { username, password });


                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadAsStringAsync();

                    HttpContext.Response.Cookies.Append("token", token); //lưu vào cookie

                    var accResponse = await _client.GetStringAsync($"https://localhost:7215/api/Accounts/get-account-id?usn={username}");

                    var account = JsonConvert.DeserializeObject<Account>(accResponse);


                    // Tạo danh sách các claim
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, account.Username), // Sử dụng tên người dùng từ cơ sở dữ liệu

                    };

                    // Tạo một ClaimsIdentity với các claims và scheme xác thực
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Tạo một ClaimsPrincipal với ClaimsIdentity
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    // Đăng nhập người dùng
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);



                    return RedirectToAction("Index", "Home");
                }

                else
                {
                    // Đăng nhập thất bại, xử lý phản hồi từ API
                    ModelState.AddModelError(string.Empty, "Đăng nhập không thành công");
                    return RedirectToAction("Login");
                }

            }
            //return View(await _context.Accounts.ToListAsync());
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp([Bind("Username,Password,PhoneNumber,Address")] Account account)
        {
            if (ModelState.IsValid)
            {
                string requestUrl = $"https://localhost:7215/api/Accounts/sign-up-acc";


                var response = await _client.PostAsJsonAsync(requestUrl, account);

                if (response.IsSuccessStatusCode)
                {               
                    TempData["SuccessMessage"] = "Create account successfully !";

                    return RedirectToAction(nameof(Login));
                }

                else
                {
                    return RedirectToAction(nameof(SignUp));
                }




            }
            return View(account);
        }

        public IActionResult Logout()
        {
            // Xóa thông tin xác thực của người dùng
            HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");
    }
    }
}
