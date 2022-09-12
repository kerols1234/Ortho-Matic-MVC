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
        public async Task<IActionResult> Upsert(int? id)
        {
            var doctor = new Doctor();
            if (id != null)
            {
                doctor = await _context.Doctors.Include(obj => obj.DoctorClinics).Include(obj => obj.DoctorHospitals).FirstOrDefaultAsync(obj => obj.Id == id);
            }

            return View(new AddDoctorVM()
            {
                Doctor = doctor,
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Doctor model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id != 0)
                {
                    if (!_context.Doctors.Any(obj => obj.Id == model.Id))
                        return NotFound();

                    _context.Doctors.Update(model);
                }
                else
                {
                    _context.Doctors.Add(model);
                }
                _context.SaveChanges();
                return Redirect(nameof(Index));
            }
            return View(model);
        }

        /* #region API Calls
         [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _db.employees.Include(c => c.Department).ToListAsync() });
        }
        */
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

        /*[HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetById(int id)
        {
            if (ModelState.IsValid)
            {
                return Json(new { data = await _db.employees.Include(obj => obj.Department).FirstOrDefaultAsync(obj => obj.Id == id) });
            }
            return Json(new { success = false, message = "Error while get data" });
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Update([FromBody] Employee model)
        {
            if (ModelState.IsValid)
            {
                var employee = _db.employees.FirstOrDefault(obj => obj.Id == model.Id);

                if (employee == null)
                {
                    return Json(new { success = false, message = "Error while updating" });
                }

                employee.Insurance = model.Insurance;
                employee.JobTitle = model.JobTitle;
                employee.EnglishName = model.EnglishName;
                employee.Email = model.Email;
                employee.Code = model.Code;
                employee.DepartmentId = model.DepartmentId;
                employee.Department = model.Department;
                employee.ArabicName = model.ArabicName;

                _db.SaveChanges();

                return Json(new { success = true, message = "update successfull" });
            }
            return Json(new { success = false, message = "Error while updating" });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Insert([FromBody] Employee model)
        {
            if (ModelState.IsValid && model.Id == 0)
            {
                _db.employees.Add(model);
                _db.SaveChanges();
                return Json(new { success = true, message = "insert successfull" });
            }
            return Json(new { success = false, message = "Error while adding" });
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _db.employees.FirstOrDefaultAsync(obj => obj.Id == id);
            if (user == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _db.employees.Remove(user);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete successfull" });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var user = await _db.employees.FirstOrDefaultAsync(obj => obj.Id == id);
            if (user == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _db.employees.Remove(user);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete successfull" });
        }

        #endregion*/
    }
}
