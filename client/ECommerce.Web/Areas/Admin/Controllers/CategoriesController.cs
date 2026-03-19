using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoriesController : Controller
{
    public IActionResult Index() => View();
}
