using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class OrdersController : Controller
{
    public IActionResult Index() => View();
}
