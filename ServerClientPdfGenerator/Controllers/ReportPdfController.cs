using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ServerClientPdfGenerator.ViewModels;

namespace ServerClientPdfGenerator.Controllers;

[Route("[controller]")]
public class ReportPdfController : Controller
{
    [HttpGet("[action]")]
    public IActionResult ReportTest()
    {
        return View(new ReportModel() { UserDto = new UserModel() { FullName = "User Test", StoreName = "My Store"}});
    }
}