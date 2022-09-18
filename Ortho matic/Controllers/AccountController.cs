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
        public async Task<IActionResult> Upsert(string id)
        {
            UpsertVM registerViewModel = new UpsertVM();
            if (id != null)
            {
                var user = await _db.ApplicationUsers.FirstOrDefaultAsync(obj => obj.Id == id);

                registerViewModel.Id = user.Id;
                registerViewModel.Name = user.UserName;
                registerViewModel.Email = user.Email;
                registerViewModel.EmployeeName = user.EmployeeName;
                registerViewModel.PhoneNumber = user.PhoneNumber;

            }
            return View(registerViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(UpsertVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Name,
                    Email = model.Email,
                    EmployeeName = model.EmployeeName,
                    PhoneNumber = model.PhoneNumber,
                };

                IdentityResult result;

                if (model.Id != null && model.Id != "")
                {
                    var oldUser = await _userManager.FindByIdAsync(model.Id) as ApplicationUser;
                    if (oldUser == null)
                    {
                        ModelState.AddModelError(string.Empty, "This user doe not exist");
                        RedirectToAction(nameof(Index));
                    }

                    if (await _userManager.IsInRoleAsync(oldUser, "Admin") && model.Name != "admin")
                    {
                        ModelState.AddModelError(string.Empty, "can not change user name of admin");
                        return View(model); ;
                    }

                    var code = await _userManager.GeneratePasswordResetTokenAsync(oldUser);
                    result = await _userManager.ResetPasswordAsync(oldUser, code, model.Password);

                    if (!result.Succeeded)
                    {
                        AddErrors(result);
                    }
                    oldUser.Email = model.Email;
                    oldUser.UserName = model.Name;
                    oldUser.PhoneNumber = model.PhoneNumber;
                    oldUser.EmployeeName = model.EmployeeName;
                    result = await _userManager.UpdateAsync(oldUser);
                }
                else
                {
                    result = await _userManager.CreateAsync(user, model.Password);
                }

                if (result.Succeeded)
                {
                    return Redirect(nameof(Index));
                }
                AddErrors(result);
            }

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
                data = await _db.ApplicationUsers.Select(obj => new UpsertVM()
                {
                    Id = obj.Id,
                    Email = obj.Email,
                    EmployeeName = obj.EmployeeName,
                    Name = obj.UserName,
                    PhoneNumber = obj.PhoneNumber
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
