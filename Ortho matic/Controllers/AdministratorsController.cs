using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ortho_matic.Data;
using Ortho_matic.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Ortho_matic.Controllers
{
    public class AdministratorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdministratorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? id)
        {
            Administrator administrator = new Administrator() { Name = ""};

            if (id != null)
            {
                administrator = _context.Administrators.FirstOrDefault(obj => obj.Id == id);
            }

            ViewBag.Regions = _context.Regions.ToList();

            return View(administrator);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var administrator = await _context.Administrators
                .FirstOrDefaultAsync(m => m.Id == id);

            if (administrator == null)
            {
                return NotFound();
            }

            ViewBag.Regions = _context.Regions.ToList();

            return View(administrator);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult insert(Administrator model)
        {
            if (ModelState.IsValid)
            {
                _context.Administrators.Add(model);
                _context.SaveChanges();
            }
            return Redirect(nameof(Index));
        }

        [HttpPost]
        public IActionResult Update([FromBody] Administrator model)
        {
            if (ModelState.IsValid)
            {
                _context.Administrators.Update(model);
                _context.SaveChanges();
                return Json(new { success = true, message = "update successfull" });
            }
            return Json(new { success = false, message = ModelState.Values });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAdministrators()
        {
            return Json(new
            {
                data = await _context.Administrators.Select(obj => new
                {
                    id = obj.Id,
                    name = obj.Name,
                    area = obj.Region != null ? obj.Region.Name : "no area selected",
                    phoneNumber = obj.PhoneNumber,
                    specialty = obj.Specialty
                }).ToListAsync()
            });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAdministrators(int id)
        {
            var adm = await _context.Administrators.FirstOrDefaultAsync(obj => obj.Id == id);
            if (adm == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _context.Administrators.Remove(adm);
            _context.SaveChanges();
            return Json(new { success = true, message = "Delete successfull" });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult AddAdministrator([FromBody] Administrator model)
        {
            if (ModelState.IsValid)
            {
                var claim = User.Claims.FirstOrDefault(obj => obj.Type == "UserName");

                if (claim == null)
                {
                    return BadRequest("Wrong User");
                }

                var user = _context.ApplicationUsers.FirstOrDefault(obj => obj.UserName == claim.Value);

                if (user == null)
                {
                    return BadRequest("This user doe not exist");
                }

                if (model.Name == null || model.Name.Trim() == "")
                {
                    return BadRequest("Administrator must contain name");
                }

                model.RegionId = user.RegionId;

                _context.Administrators.Add(model);
                _context.SaveChanges();
                return Ok(new { id = model.Id });
            }
            return BadRequest(ModelState.Values.ToString());
        }

    }
}
