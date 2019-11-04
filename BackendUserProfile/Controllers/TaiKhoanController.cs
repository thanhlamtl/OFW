using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PhuKienDienThoai.Models;
using PhuKienDienThoai.Models.AccountViewModels;

namespace PhuKienDienThoai.Areas.Users.Controllers
{
    [Area("Users")]
    [Authorize(Roles = "Admin, User")]
    public class TaiKhoanController : Controller
    {
        UserManager<ApplicationUser> usermanager;
        public TaiKhoanController(UserManager<ApplicationUser> _userManager) => usermanager = _userManager;
        public async Task<IActionResult> Index()
        {
            var user = await usermanager.GetUserAsync(HttpContext.User);
            var userView = new UserViewModel
            {
                HoTen = user.HoTen,
                Email = user.Email,
                DiaChi = user.DiaChi,
                GioiTinh = user.GioiTinh,
                NgaySinh = user.NgaySinh,
                PhoneNumber = user.PhoneNumber,
            };
            return View(userView);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTaiKhoan(UserViewModel user)
        {
            //get user cũ
            var oldUser = await usermanager.GetUserAsync(HttpContext.User);
            if (oldUser.Email != user.Email)
            {
                //nếu người dùng muốn đổi địa chỉ email thì không cho đổi
                TempData["Message"] = "Không được đổi địa chỉ email";
                return RedirectToAction(actionName: "Index");
            }
            oldUser.HoTen = user.HoTen;
            oldUser.DiaChi = user.DiaChi;
            oldUser.GioiTinh = user.GioiTinh;
            oldUser.NgaySinh = user.NgaySinh;
            oldUser.PhoneNumber = user.PhoneNumber;
            var update = await usermanager.UpdateAsync(oldUser);
            if (update.Succeeded)
                TempData["Message"] = "Cập nhật thành công";
            else
            {
                if (update.Errors != null)
                    Logs.Logger.Log(update.Errors.ToString());
                TempData["Message"] = "Cập nhật thất bại";
            }
            return RedirectToAction(actionName: "Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                usermanager.Dispose();
        }
    }
}