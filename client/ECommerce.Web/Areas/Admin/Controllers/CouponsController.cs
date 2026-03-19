using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class CouponsController : Controller
{
    public IActionResult Index() => View();
}
