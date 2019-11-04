using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhuKienDienThoai.Controllers;
using PhuKienDienThoai.Models;
using PhuKienDienThoai.Models.ManageViewModels;
using PhuKienDienThoai.Services;

namespace PhuKienDienThoai.Areas.Users.Controllers
{
    [Area("Users")]
    [Authorize(Roles = "Admin, User")]
    public class MatKhauController : Controller
    {
         UserManager<ApplicationUser> usermanager;
         SignInManager<ApplicationUser> _signInManager;
         IEmailSender _emailSender;
         ILogger<ManageController> _logger;
         UrlEncoder _urlEncoder;
        IHostingEnvironment  _enviroment;

        public MatKhauController(
            UserManager<ApplicationUser> _usermanager,
            SignInManager<ApplicationUser> signInManager,
            IHostingEnvironment  environment,
            IEmailSender emailSender,
            ILogger<ManageController> logger,
            UrlEncoder urlEncoder)
        {
            usermanager = _usermanager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _urlEncoder = urlEncoder;
            _enviroment = environment;
        }
        protected override void Dispose(bool disposing)
        {
            if(disposing)
                usermanager.Dispose();
        }
        public IActionResult Index() => View();
        public IActionResult DoiMatKhau()
        {
            var changePassword = new ChangePasswordViewModel();
            return View(changePassword);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var user = await usermanager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{usermanager.GetUserId(User)}'.");
            var kiemtraMatKhauCu = await usermanager.CheckPasswordAsync(user, model.OldPassword);
            if (!kiemtraMatKhauCu)
                TempData["Message"] = "Mật khẩu cũ không khớp, vui lòng nhập lại";
            var changePasswordResult = await usermanager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                Logs.Logger.Log(changePasswordResult.ToString());
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation(message: "User changed their password successfully.");
            TempData["Message"] = "Thay đổi mật khẩu thành công.";
            return RedirectToAction(nameof(DoiMatKhau));
        }
    }
}