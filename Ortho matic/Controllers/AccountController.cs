using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Ortho_matic.Data;
using Ortho_matic.Models;
using Ortho_matic.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ortho_matic.Controllers
{
    [Authorize]
    [EnableCors]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;

        public AccountController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext db, IConfiguration config)
        {
            _roleManager = roleManager;
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
            model.UserRole = (await _userManager.GetRolesAsync(user))[0];

            ViewBag.Regions = _db.Regions.ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateVM model)
        {
            if (ModelState.IsValid)
            {
                var loginUser = await _userManager.GetUserAsync(User);

                IdentityResult result;

                var oldUser = await _userManager.FindByIdAsync(model.Id) as ApplicationUser;

                if (oldUser == null)
                {
                    ModelState.AddModelError(string.Empty, "This user doe not exist");
                    RedirectToAction(nameof(Index));
                }

                if (await _userManager.IsInRoleAsync(oldUser, "Admin") && await _userManager.IsInRoleAsync(loginUser, "SubAdmin"))
                {
                    ModelState.AddModelError(string.Empty, "can not change admin");
                    return View(model);
                }

                if (await _userManager.IsInRoleAsync(oldUser, "SubAdmin") && await _userManager.IsInRoleAsync(loginUser, "SubAdmin") && oldUser.Id != loginUser.Id)
                {
                    ModelState.AddModelError(string.Empty, "can not change user have the same role");
                    return View(model);
                }

                if (model.UserRole == "SubAdmin" && await _userManager.IsInRoleAsync(loginUser, "SubAdmin"))
                {
                    ModelState.AddModelError(string.Empty, "can not change user role to this role");
                    model.UserRole = "SubAdmin";
                    return View(model);
                }

                if (await _userManager.IsInRoleAsync(oldUser, "Admin") && model.Name != "admin")
                {
                    ModelState.AddModelError(string.Empty, "can not change user name of admin");
                    return View(model);
                }

                if (await _userManager.IsInRoleAsync(oldUser, "Admin"))
                {
                    var oldRole = await _userManager.GetRolesAsync(loginUser);
                    await _userManager.RemoveFromRoleAsync(oldUser, oldRole[0]);
                    await _userManager.AddToRoleAsync(oldUser, model.UserRole);
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
                var loginUser = await _userManager.GetUserAsync(User);
                var loginRole = (await _userManager.GetRolesAsync(loginUser))[0];

                var user = new ApplicationUser
                {
                    UserName = model.Name,
                    Email = model.Email,
                    EmployeeName = model.EmployeeName,
                    PhoneNumber = model.PhoneNumber,
                    RegionId = model.RegionId
                };

                if (loginRole == "Staff")
                {
                    ModelState.AddModelError(string.Empty, "can add users");
                    ViewBag.Regions = _db.Regions.ToList();
                    return View(model);
                }

                var result = await _userManager.CreateAsync(user, model.Password);

                if (loginRole == "Admin")
                {
                    await _userManager.AddToRoleAsync(user, model.UserRole);
                }

                if (loginRole == "SubAdmin")
                {
                    await _userManager.AddToRoleAsync(user, "Staff");
                }

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
                var user = await _db.ApplicationUsers.FirstOrDefaultAsync(obj => obj.UserName == model.UserName);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid userName.");
                    return View(model);
                }

                if (await _userManager.IsInRoleAsync(user, "Staff"))
                {
                    ModelState.AddModelError(string.Empty, "can not login.");
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
            var loginUser = await _userManager.GetUserAsync(User);
            var loginRole = (await _userManager.GetRolesAsync(loginUser))[0];

            var list = await _db.ApplicationUsers.Select(obj => new InsertVM()
            {
                Id = obj.Id,
                Email = obj.Email,
                EmployeeName = obj.EmployeeName,
                Name = obj.UserName,
                PhoneNumber = obj.PhoneNumber,
                Region = obj.Region != null ? obj.Region.Name : "no area selected",
            }).ToListAsync();

            if(loginRole != "Admin")
            {
                list.Remove(list.First(obj => obj.Name == "admin"));
            }

            return Json(new
            {
                data = list,
            });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var loginUser = await _userManager.GetUserAsync(User);
            var loginRole = (await _userManager.GetRolesAsync(loginUser))[0];

            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(obj => obj.Id == id);

            if (user == null)
            {
                return Json(new { success = false, message = "No user with this id" });
            }
            if (user.Id == loginUser.Id)
            {
                return Json(new { success = false, message = "can not delete your self" });
            }
            if (loginRole == "SubAdmin" && await _userManager.IsInRoleAsync(user, "SubAdmin"))
            {
                return Json(new { success = false, message = "can not delete user have same role" });
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

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithJWT([FromBody] LoginVM login)
        {
            IActionResult response = Unauthorized("Wrong email or password");
            var user = await _userManager.FindByNameAsync(login.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, login.Password))
            {
                var token = GenerateJSONWebTokenAsync(user.UserName);
                response = Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(await token),
                    expiration = TimeZoneInfo.ConvertTimeBySystemTimeZoneId((await token).ValidTo, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt"),
                    userDate = await userInfoAsync(user.UserName)
                });
            }

            return response;
        }

        private async Task<JwtSecurityToken> GenerateJSONWebTokenAsync(string userName)
        {
            var claims = new List<Claim> { new Claim("UserName", userName) };

            var user = await _userManager.FindByNameAsync(userName);

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["JWT:ValidIssuer"],
              _config["JWT:ValidAudience"],
              claims,
              expires: DateTime.Now.AddHours(8),
              signingCredentials: credentials);

            return token;
        }

        private async Task<object> userInfoAsync(string userName)
        {
            var user = await _db.ApplicationUsers.Include(obj => obj.Region).FirstOrDefaultAsync(obj => obj.UserName == userName);
            if (user != null)
            {
                return new
                {
                    user.UserName,
                    user.Email,
                    user.PhoneNumber,
                    user.EmployeeName,
                    user.RegionId,
                    Region = user.Region.Name,
                };
            }

            return "Wrong User Name";
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateFromMobile([FromBody] UpdateVM model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result;

                var claim = User.Claims.FirstOrDefault(obj => obj.Type == "UserName");

                if (claim == null)
                {
                    return Ok(new { success = false, message = "Wrong User" });
                }

                var oldUser = _db.ApplicationUsers.Include(obj => obj.Region).FirstOrDefault(obj => obj.UserName == claim.Value);

                if (oldUser == null)
                {
                    return Ok(new { success = false, message = "This user doe not exist" });
                }

                if (model.Password != null && model.Password.Trim() != "")
                {
                    var code = await _userManager.GeneratePasswordResetTokenAsync(oldUser);
                    result = await _userManager.ResetPasswordAsync(oldUser, code, model.Password);

                    if (!result.Succeeded)
                    {
                        return Ok(new { success = false, message = result.Errors.ElementAt(0).Description });
                    }
                }

                if (model.Email != null && model.Email.Trim() != "")
                {
                    oldUser.Email = model.Email;
                }

                if (model.PhoneNumber != null && model.PhoneNumber.Trim() != "")
                {
                    oldUser.PhoneNumber = model.PhoneNumber;
                }

                if (model.EmployeeName != null && model.EmployeeName.Trim() != "")
                {
                    oldUser.EmployeeName = model.EmployeeName;
                }

                result = await _userManager.UpdateAsync(oldUser);

                if (result.Succeeded)
                {
                    var token = await GenerateJSONWebTokenAsync(oldUser.UserName);
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(token.ValidTo, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt"),
                        userDate = await userInfoAsync(oldUser.UserName)
                    });
                }

                return Ok(new { success = false, message = result.Errors.ElementAt(0).Description });
            }
            return Ok(new { success = false, message = "Error while updating" });
        }
    }
}
