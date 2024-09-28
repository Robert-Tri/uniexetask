using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uniexetask.web.Models;

namespace uniexetask.web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public UserController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient("UniApiClient");
        }

        [HttpGet]
        public async Task<IActionResult> GetUserList()
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.GetAsync("user");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ResponseViewModel<string>>();
                    if (result != null && !string.IsNullOrEmpty(result.Data))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
            }
            return View();
        }
    }
}
