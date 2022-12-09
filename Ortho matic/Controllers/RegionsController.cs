using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ortho_matic.Data;
using Ortho_matic.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ortho_matic.Controllers
{
    [Authorize]
    [EnableCors]
    public class RegionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? id)
        {
            var region = new Region();
            if (id != null)
            {
                region = _context.Regions.FirstOrDefault(obj => obj.Id == id);
            }
            return View(region);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Region model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id != 0)
                {
                    _context.Regions.Update(model);
                }
                else
                {
                    _context.Regions.Add(model);
                }
                _context.SaveChanges();
            }
            return Redirect(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRegions()
        {
            return Json(new
            {
                data = await _context.Regions.Select(obj => new
                {
                    obj.Id,
                    obj.Name,
                    NumberOfHospitals = _context.Hospitals.Count(obj1 => obj1.RegionId == obj.Id),
                    NumberOfClinics = _context.Clinics.Count(obj1 => obj1.RegionId == obj.Id),
                    NumberOfUsers = _context.ApplicationUsers.Count(obj1 => obj1.RegionId == obj.Id)
                }).ToListAsync()
            });
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetAllRegionsMobile()
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

            var list = _context
                .Regions
                .Select(obj => new
                {
                    obj.Name,
                    obj.Id
                })
                .ToList();

            return Ok(list);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRegions(int id)
        {
            try
            {
                var region = await _context.Regions.FirstOrDefaultAsync(obj => obj.Id == id);
                if (region == null)
                {
                    return Json(new { success = false, message = "Error while deleting" });
                }
                _context.Regions.Remove(region);
                _context.SaveChanges();
                return Json(new { success = true, message = "Delete successfull" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }

        }

    }
}
