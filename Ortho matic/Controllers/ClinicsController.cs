﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ortho_matic.Data;
using Ortho_matic.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Ortho_matic.Controllers
{
    public class ClinicsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClinicsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? id)
        {
            Clinic clinic = new Clinic() { Address = "" };
            if (id != null)
            {
                clinic = _context.Clinics.FirstOrDefault(obj => obj.Id == id);
            }
            return View(clinic);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clinic = await _context.Clinics.Include(obj => obj.DoctorClinics)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clinic == null)
            {
                return NotFound();
            }

            return View(clinic);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Clinic model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id != 0)
                {
                    _context.Clinics.Update(model);
                }
                else
                {
                    _context.Clinics.Add(model);
                }
                _context.SaveChanges();
                return Redirect(nameof(Index));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllClinics()
        {
            return Json(new
            {
                data = await _context.Clinics.Include(obj => obj.DoctorClinics).Select(obj => new
                {
                    Id = obj.Id,
                    PhoneNumber = obj.PhoneNumber,
                    Address = obj.Address,
                    NumberOfDoctors = obj.DoctorClinics.Count()
                }).ToListAsync()
            });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteClinics(int id)
        {
            var clinic = await _context.Clinics.FirstOrDefaultAsync(obj => obj.Id == id);
            if (clinic == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _context.Clinics.Remove(clinic);
            _context.SaveChanges();
            return Json(new { success = true, message = "Delete successfull" });
        }
    }
}
