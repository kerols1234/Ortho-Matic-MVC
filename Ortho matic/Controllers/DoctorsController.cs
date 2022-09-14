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
                return Redirect(nameof(Details));
            }
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var dpctor = await _context.Doctors.Include(obj => obj.DoctorClinics).Include(obj => obj.DoctorHospitals)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dpctor == null)
            {
                return NotFound(new AddDoctorVM()
                {
                    DoctorDegreeSelectList = Enum.GetNames(typeof(Degree)).Select(i => new SelectListItem
                    {
                        Text = i.ToString(),
                        Value = i.ToString(),
                    }).AsQueryable(),
                    DoctorSpecialtySelectList = Enum.GetNames(typeof(Specialty)).Select(i => new SelectListItem
                    {
                        Text = i.ToString(),
                        Value = i.ToString(),
                    }).AsQueryable(),
                    ClinicSelectList = _context.Clinics.Select(i => new SelectListItem
                    {
                        Text = i.Address.ToString(),
                        Value = i.Id.ToString(),
                    }).AsQueryable(),
                    HospitsalSelectList = _context.Hospitals.Select(i => new SelectListItem
                    {
                        Text = i.Name.ToString(),
                        Value = i.Id.ToString(),
                    }).AsQueryable(),
                    DaySelectList = Enum.GetNames(typeof(DayOfWeekInArabic)).Select(i => new SelectListItem
                    {
                        Text = i.ToString(),
                        Value = i.ToString(),
                    }).AsQueryable()
                });
            }

            return View(dpctor);
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
            await _context.DoctorClinics.AddAsync(doctorClinic);
            _context.SaveChanges();
            return RedirectToAction(nameof(Details));
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
            await _context.DoctorHospitals.AddAsync(doctorHospital);
            _context.SaveChanges();
            return RedirectToAction(nameof(Details));
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
