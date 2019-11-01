using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PhuKienDienThoai.Data;
using PhuKienDienThoai.Models;

namespace PhuKienDienThoai.Areas.Users.ViewComponents
{
    [Authorize(Roles = "Admin, User")]
    [ViewComponent]
    public class Sidebar : ViewComponent, IDisposable
    {
        UserManager<ApplicationUser> userManager;
        public Sidebar(ApplicationDbContext _context, UserManager<ApplicationUser> _usermanager) =>
            userManager = _usermanager;

        public void Dispose() => ((IDisposable)userManager).Dispose();

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            return View(user);
        }


    }
}