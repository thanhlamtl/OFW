using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhuKienDienThoai.Data;
using PhuKienDienThoai.Models;

namespace PhuKienDienThoai.Areas.Users.Controllers
{
    [Area("Users")]
    [Authorize(Roles = "Admin, User")]
    public class WishlistController : Controller
    {
        ApplicationDbContext context;
        UserManager<ApplicationUser> userManager;

        public WishlistController(ApplicationDbContext _context, UserManager<ApplicationUser> _usermanager)
        {
            context = _context;
            userManager = _usermanager;
        }
        public async Task<IActionResult> Index(int? page)
        {

            //lấy người dùng hiện tại
            var user = await userManager.GetUserAsync(HttpContext.User);
            //lấy dữ liệu theo người dùng hiện tại
            var lstItemTrongWishlist = await context.Wishlist
                                                    .Include(x => x.SanPham)
                                                    .Include(x => x.User)
                                                    .Where(x => x.UserID == user.Id)
                                                    .ToListAsync();
            
             return View(lstItemTrongWishlist);
        }
        ///<param name="id">sản phẩm id</param>
        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> ThemVaoWishList(int id)
        {
            try
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return new JsonResult(new
                    {
                        error = 1,
                        message = "Bạn phải đăng nhập trước khi thêm vào wishlist",
                    });
                }
                var kiemtraDaTonTai = await context.Wishlist.FindAsync(id, user.Id);
                if (kiemtraDaTonTai == null)
                {
                    await context.Wishlist.AddAsync(new Wishlist
                    {
                        SanPhamID = id,
                        User = await userManager.GetUserAsync(HttpContext.User),
                    });
                    context.SaveChanges();
                }
                return Ok();
            }
            catch (System.Exception ex)
            {
                Logs.Logger.Log(ex.Message);
                return new JsonResult(new
                {
                    error = 1,
                    message = ex.Message,
                }); ;
            }
        }
        public async Task<IActionResult> XoaKhoiWishList(int id)
        {
            try
            {
                var currentuser = await userManager.GetUserAsync(User);
                var item_need_to_remove = await context.Wishlist.FirstOrDefaultAsync(z => z.UserID == currentuser.Id && z.SanPhamID == id);
                context.Wishlist.Remove(item_need_to_remove);
                await context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                Logs.Logger.Log(ex.Message);
                throw;
            }
            return RedirectToAction(actionName: "Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
                userManager.Dispose();
            }
        }
    }
}