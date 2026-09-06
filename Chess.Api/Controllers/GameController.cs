using Microsoft.AspNetCore.Mvc;

namespace Chess.Api.Controllers;

public class GameController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}