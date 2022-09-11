using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ortho_matic.Data;
using Ortho_matic.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Ortho_matic.Controllers
{
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
            return View(hospital);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hospital = await _context.Hospitals.Include(obj => obj.DoctorHospitals)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hospital == null)
            {
                return NotFound();
            }

            return View(hospital);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Hospital model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id != 0)
                {
                    _context.Hospitals.Update(model);
                }
                else
                {
                    _context.Hospitals.Add(model);
                }
                _context.SaveChanges();
                return Redirect(nameof(Index));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHospitals()
        {
            return Json(new
            {
                data = await _context.Hospitals.Include(obj => obj.DoctorHospitals).Select(obj => new
                {
                    Id = obj.Id,
                    Name = obj.Name,
                    Address = obj.Address,
                    PhoneNumber = obj.PhoneNumber,
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
            _context.Hospitals.Remove(hos);
            _context.SaveChanges();
            return Json(new { success = true, message = "Delete successfull" });
        }
    }
}
