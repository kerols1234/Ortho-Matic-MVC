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
            Hospital hospital = new Hospital() { Name = "" };
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

            var hospital = await _context.Hospitals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hospital == null)
            {
                return NotFound();
            }

            return View(hospital);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Latitude,Longitude")] Hospital hospital)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hospital);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hospital);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hospital = await _context.Hospitals.FindAsync(id);
            if (hospital == null)
            {
                return NotFound();
            }
            return View(hospital);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Latitude,Longitude")] Hospital hospital)
        {
            if (id != hospital.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hospital);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HospitalExists(hospital.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
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


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hospital = await _context.Hospitals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hospital == null)
            {
                return NotFound();
            }

            return View(hospital);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hospital = await _context.Hospitals.FindAsync(id);
            _context.Hospitals.Remove(hospital);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HospitalExists(int id) => _context.Hospitals.Any(e => e.Id == id);

        [HttpGet]
        public async Task<IActionResult> GetAllHospitals()
        {
            return Json(new { data = await _context.Hospitals.ToListAsync() });
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
