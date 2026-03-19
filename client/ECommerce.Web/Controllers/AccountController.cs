using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Web.Controllers;

public class AccountController : Controller
{
    public IActionResult Login() => View();
    public IActionResult Register() => View();
}
