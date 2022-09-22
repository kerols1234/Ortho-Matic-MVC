using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ortho_matic.Data;
using Ortho_matic.Models;
using Ortho_matic.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Ortho_matic.Controllers
{
    [Authorize]
    [EnableCors]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext db, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _db.ApplicationUsers.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            UpdateVM model = new UpdateVM();
            model.Id = user.Id;
            model.Name = user.UserName;
            model.Email = user.Email;
            model.EmployeeName = user.EmployeeName;
            model.PhoneNumber = user.PhoneNumber;
            model.RegionId = user.RegionId;

            ViewBag.Regions = _db.Regions.ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateVM model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result;

                var oldUser = await _userManager.FindByIdAsync(model.Id) as ApplicationUser;

                if (oldUser == null)
                {
                    ModelState.AddModelError(string.Empty, "This user doe not exist");
                    RedirectToAction(nameof(Index));
                }

                if (await _userManager.IsInRoleAsync(oldUser, "Admin") && model.Name != "admin")
                {
                    ModelState.AddModelError(string.Empty, "can not change user name of admin");
                    return View(model);
                }

                if (model.Password != null && model.Password.Trim() != "")
                {
                    var code = await _userManager.GeneratePasswordResetTokenAsync(oldUser);
                    result = await _userManager.ResetPasswordAsync(oldUser, code, model.Password);

                    if (!result.Succeeded)
                    {
                        AddErrors(result);
                    }
                }

                if (model.Email != null && model.Email.Trim() != "")
                {
                    oldUser.Email = model.Email;
                }

                if (model.Name != null && model.Name.Trim() != "")
                {
                    oldUser.UserName = model.Name;
                }

                if (model.PhoneNumber != null && model.PhoneNumber.Trim() != "")
                {
                    oldUser.PhoneNumber = model.PhoneNumber;
                }

                if (model.EmployeeName != null && model.EmployeeName.Trim() != "")
                {
                    oldUser.EmployeeName = model.EmployeeName;
                }

                if (model.RegionId != null)
                {
                    oldUser.RegionId = model.RegionId == -1 ? null : model.RegionId;
                }

                result = await _userManager.UpdateAsync(oldUser);

                if (result.Succeeded)
                {
                    return Redirect(nameof(Index));
                }
                AddErrors(result);
            }

            ViewBag.Regions = _db.Regions.ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult Insert()
        {
            ViewBag.Regions = _db.Regions.ToList();

            return View(new InsertVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Insert(InsertVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Name,
                    Email = model.Email,
                    EmployeeName = model.EmployeeName,
                    PhoneNumber = model.PhoneNumber,
                    RegionId = model.RegionId
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return Redirect(nameof(Index));
                }
                AddErrors(result);
            }

            ViewBag.Regions = _db.Regions.ToList();

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                if (model.UserName != "admin")
                {
                    ModelState.AddModelError(string.Empty, "only Admin can login");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            return Json(new
            {
                data = await _db.ApplicationUsers.Select(obj => new InsertVM()
                {
                    Id = obj.Id,
                    Email = obj.Email,
                    EmployeeName = obj.EmployeeName,
                    Name = obj.UserName,
                    PhoneNumber = obj.PhoneNumber,
                    Region = obj.Region != null ? obj.Region.Name : "no region selected",
                }).ToListAsync()
            });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(obj => obj.Id == id);
            if (user == null)
            {
                return Json(new { success = false, message = "No user with this id" });
            }
            if (user.UserName == "admin")
            {
                return Json(new { success = false, message = "Can not delete admin" });
            }
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Json(new { success = true, message = "Delete successfull" });
            }
            return Json(new { success = false, message = "Error while deleting" });
        }
    }
}
