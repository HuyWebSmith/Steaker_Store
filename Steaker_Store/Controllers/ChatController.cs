using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Steaker_Store.Models;
using System.Linq;
using System.Threading.Tasks;

public class ChatController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string targetUser)
    {
        if (User.IsInRole("Admin"))
        {
            var customers = await _userManager.GetUsersInRoleAsync("Customer");
            ViewBag.CustomerList = customers;
        }
        if (User.IsInRole("Customer"))
        {
            var admin = _userManager.GetUsersInRoleAsync("Admin").Result.FirstOrDefault();
            var adminEmail = admin?.Email;
            ViewBag.AdminEmail = adminEmail;

        }

        ViewBag.TargetUser = targetUser;
        return View();
    }


}
