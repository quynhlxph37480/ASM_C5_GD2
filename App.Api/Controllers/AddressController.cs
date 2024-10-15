using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using App.Data.Entities;

namespace App.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AddressController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            var token = "814a080c-256d-11ef-9e93-f2508e67c133"; // Thay thế bằng cách lấy token từ nguồn lưu trữ bảo mật
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("token", token);

            var response = await client.GetStringAsync("https://online-gateway.ghn.vn/shiip/public-api/master-data/province");

            return Ok(response);
        }

        [HttpGet("districts")]
        public async Task<IActionResult> GetDistricts(string provinceId)
        {
            var token = "814a080c-256d-11ef-9e93-f2508e67c133"; // Thay thế bằng cách lấy token từ nguồn lưu trữ bảo mật
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("token", token);

            var response = await client.GetStringAsync($"https://online-gateway.ghn.vn/shiip/public-api/master-data/district?province_id={provinceId}");

            return Ok(response);
        }

        [HttpGet("communes")]
        public async Task<IActionResult> GetCommunes(string districtId)
        {
            var token = "814a080c-256d-11ef-9e93-f2508e67c133"; // Thay thế bằng cách lấy token từ nguồn lưu trữ bảo mật
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("token", token);

            var response = await client.GetStringAsync($"https://online-gateway.ghn.vn/shiip/public-api/master-data/ward?district_id={districtId}");

            return Ok(response);
        }
    }
}

