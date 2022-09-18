﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ortho_matic.Data;
using Ortho_matic.Models;
using Ortho_matic.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ortho_matic.Controllers
{
    [Authorize]
    [EnableCors]
    public class DoctorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(new DoctorVM
            {
                Doctor = new Doctor(),
                DoctorDegreeSelectList = Enum.GetNames(typeof(Degree)).Select(i => new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                }).AsQueryable(),
                DoctorSpecialtySelectList = Enum.GetNames(typeof(Specialty)).Select(i => new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                }).AsQueryable()
            });
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Doctor model)
        {
            if (ModelState.IsValid)
            {
                _context.Doctors.Add(model);
                _context.SaveChanges();
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit([FromBody] Doctor model)
        {
            if (ModelState.IsValid)
            {
                _context.Doctors.Update(model);
                _context.SaveChanges();
                return Json(new { success = true, message = "Edit successfull" });
            }
            return Json(new { success = false, message = ModelState.Values });
        }


        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(obj => obj.DoctorClinics)
                .ThenInclude(obj => obj.Clinic)
                .Include(obj => obj.DoctorClinics)
                .ThenInclude(obj => obj.BestTimeForVisit)
                .Include(obj => obj.DoctorHospitals)
                .ThenInclude(obj => obj.Hospital)
                .Include(obj => obj.DoctorHospitals)
                .ThenInclude(obj => obj.BestTimeForVisit)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (doctor == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(doctor);
        }

        public IActionResult AddClinic(int id)
        {
            ViewData["id"] = id;
            var ClinicSelectList = _context.Clinics.Select(i => new SelectListItem
            {
                Text = i.Address.ToString(),
                Value = i.Id.ToString(),
            }).ToList();
            return View(ClinicSelectList);
        }

        [HttpPost]
        public async Task<IActionResult> AddClinic([FromBody] DoctorClinic doctorClinic)
        {
            try
            {
                await _context.DoctorClinics.AddAsync(doctorClinic);
                var re = _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return RedirectToAction(nameof(Details), new { id = doctorClinic.DoctorId });
        }

        public IActionResult AddHospital(int id)
        {
            ViewData["id"] = id;
            var HospitsalSelectList = _context.Hospitals.Select(i => new SelectListItem
            {
                Text = i.Name.ToString(),
                Value = i.Id.ToString(),
            }).ToList();
            return View(HospitsalSelectList);
        }

        [HttpPost]
        public async Task<IActionResult> AddHospital([FromBody] DoctorHospital doctorHospital)
        {
            try
            {
                await _context.DoctorHospitals.AddAsync(doctorHospital);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return RedirectToAction(nameof(Details), new { id = doctorHospital.DoctorId });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            return Json(new
            {
                data = await _context.Doctors.Select(obj => new
                {
                    obj.PhoneNumber,
                    obj.Name,
                    DoctorSpecialty = obj.DoctorSpecialty.ToString(),
                    obj.Id,
                    DoctorDegree = obj.DoctorDegree.ToString(),
                }).ToListAsync()
            });
        }
    }
}
