using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class CustomersController : Controller
{
    public IActionResult Index() => View();
}
