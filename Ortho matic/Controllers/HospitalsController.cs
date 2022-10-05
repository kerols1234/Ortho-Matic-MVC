using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ortho_matic.Data;
using Ortho_matic.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Ortho_matic.Controllers
{
    [Authorize]
    [EnableCors]
    public class HospitalsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HospitalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? id)
        {
            Hospital hospital = new Hospital() { Name = "", Address = "" };

            if (id != null)
            {
                hospital = _context.Hospitals.FirstOrDefault(obj => obj.Id == id);
            }

            ViewBag.Regions = _context.Regions.ToList();

            return View(hospital);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var hospital = await _context.Hospitals
                .Include(obj => obj.DoctorHospitals)
                .ThenInclude(obj => obj.BestTimeForVisit)
                .Include(obj => obj.DoctorHospitals)
                .ThenInclude(obj => obj.Doctor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hospital == null)
            {
                return NotFound();
            }

            ViewBag.Regions = _context.Regions.ToList();

            return View(hospital);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult insert(Hospital model)
        {
            if (ModelState.IsValid)
            {
                _context.Hospitals.Add(model);
                _context.SaveChanges();
            }
            return Redirect(nameof(Index));
        }

        [HttpPost]
        public IActionResult Update([FromBody] Hospital model)
        {
            if (ModelState.IsValid)
            {
                _context.Hospitals.Update(model);
                _context.SaveChanges();
                return Json(new { success = true, message = "update successfull" });
            }
            return Json(new { success = false, message = ModelState.Values });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHospitals()
        {
            return Json(new
            {
                data = await _context.Hospitals.Include(obj => obj.DoctorHospitals).Select(obj => new
                {
                    obj.Id,
                    obj.Name,
                    Region = obj.Region != null ? obj.Region.Name : "no region selected",
                    obj.Address,
                    obj.Phone1,
                    NumberOfDoctors = obj.DoctorHospitals.Count()
                }).ToListAsync()
            });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteHospitals(int id)
        {
            var hos = await _context.Hospitals.FirstOrDefaultAsync(obj => obj.Id == id);
            if (hos == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            var doctorHospital = _context.DoctorHospitals.Include(obj => obj.BestTimeForVisit).Include(obj => obj.Times).Where(obj => obj.HospitalId == id);
            _context.Times.RemoveRange(doctorHospital.Select(obj => obj.BestTimeForVisit));
            _context.Times.RemoveRange(doctorHospital.SelectMany(obj => obj.Times));
            _context.DoctorHospitals.RemoveRange(doctorHospital);
            _context.Hospitals.Remove(hos);
            _context.SaveChanges();
            return Json(new { success = true, message = "Delete successfull" });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult AddHospital([FromBody] Hospital model)
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
                    return BadRequest("Hospital must contain name");
                }

                if (model.Address == null || model.Address.Trim() == "")
                {
                    return BadRequest("Hospital must contain address");
                }

                model.RegionId = user.RegionId;

                _context.Hospitals.Add(model);
                _context.SaveChanges();
                return Ok(new { id = model.Id });
            }
            return BadRequest(ModelState.Values.ToString());
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetAllHospitalsMobile()
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
                .Hospitals
                .Where(obj => obj.RegionId == user.RegionId)
                .Select(obj => new
                {
                    obj.Name,
                    obj.Id
                })
                .ToList();

            return Ok(list);
        }
    }
}
