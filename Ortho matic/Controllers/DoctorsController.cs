using Microsoft.AspNetCore.Authorization;
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

        [HttpDelete]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            try
            {
                var doctor = await _context.Doctors
                    .Include(obj => obj.DoctorClinics)
                    .ThenInclude(obj => obj.BestTimeForVisit)
                    .Include(obj => obj.DoctorClinics)
                    .ThenInclude(obj => obj.Times)
                    .Include(obj => obj.DoctorHospitals)
                    .ThenInclude(obj => obj.Times)
                    .Include(obj => obj.DoctorHospitals)
                    .ThenInclude(obj => obj.BestTimeForVisit)
                    .FirstOrDefaultAsync(obj => obj.Id == id);

                if (doctor == null)
                {
                    return Json(new { success = false, message = "Error while deleting" });
                }

                _context.Times.RemoveRange(doctor.DoctorClinics.Select(obj => obj.BestTimeForVisit));
                _context.Times.RemoveRange(doctor.DoctorClinics.SelectMany(obj => obj.Times));
                _context.Times.RemoveRange(doctor.DoctorHospitals.Select(obj => obj.BestTimeForVisit));
                _context.Times.RemoveRange(doctor.DoctorHospitals.SelectMany(obj => obj.Times));

                _context.DoctorHospitals.RemoveRange(doctor.DoctorHospitals);
                _context.DoctorClinics.RemoveRange(doctor.DoctorClinics);

                _context.Doctors.Remove(doctor);
                _context.SaveChanges();
                return Json(new { success = true, message = "Delete successfull" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }

        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDoctorClinic(int idD, int idC)
        {
            try
            {
                var doctorClinic = await _context.DoctorClinics.Include(obj => obj.BestTimeForVisit).Include(obj => obj.Times).FirstOrDefaultAsync(obj => obj.ClinicId == idC && obj.DoctorId == idD);
                if (doctorClinic == null)
                {
                    return Json(new { success = false, message = "Error while deleting" });
                }
                _context.Times.Remove(doctorClinic.BestTimeForVisit);
                _context.Times.RemoveRange(doctorClinic.Times);
                _context.DoctorClinics.Remove(doctorClinic);
                _context.SaveChanges();
                return Json(new { success = true, message = "Delete successfull" });
            }
            catch (Exception e)
            {
                var s = e.Message;
                return Json(new { success = true, message = e });
            }

        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDoctorHospital(int idD, int idH)
        {
            var doctorHospital = await _context.DoctorHospitals.Include(obj => obj.BestTimeForVisit).Include(obj => obj.Times).FirstOrDefaultAsync(obj => obj.HospitalId == idH && obj.DoctorId == idD);
            if (doctorHospital == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _context.Times.Remove(doctorHospital.BestTimeForVisit);
            _context.Times.RemoveRange(doctorHospital.Times);
            _context.DoctorHospitals.Remove(doctorHospital);
            _context.SaveChanges();
            return Json(new { success = true, message = "Delete successfull" });
        }

        [HttpGet]
        public IActionResult DetailsClinic(int idD, int idC)
        {
            var model = _context.DoctorClinics
                .Include(obj => obj.Clinic)
                .Include(obj => obj.BestTimeForVisit)
                .Include(obj => obj.Times)
                .FirstOrDefault(obj => obj.ClinicId == idC && obj.DoctorId == idD);
            return View(model);
        }

        [HttpPost]
        public IActionResult EditClinic([FromBody] DoctorClinic doctorClinic)
        {
            try
            {
                var oldDoctorClinic = _context.DoctorClinics
                    .Include(obj => obj.BestTimeForVisit)
                    .Include(obj => obj.Times)
                    .AsNoTracking()
                    .FirstOrDefault(obj => obj.ClinicId == doctorClinic.ClinicId && obj.DoctorId == doctorClinic.DoctorId);

                _context.Times.RemoveRange(oldDoctorClinic.Times.Except(doctorClinic.Times));

                oldDoctorClinic.Times = doctorClinic.Times;
                oldDoctorClinic.BestTimeForVisit = doctorClinic.BestTimeForVisit;

                // _context.Entry(doctorClinic).State = EntityState.Detached;
                //_context.Entry(doctorClinic.Times).State = EntityState.Detached;
                //_context.Entry(doctorClinic.BestTimeForVisit).State = EntityState.Detached;

                _context.DoctorClinics.Update(oldDoctorClinic);
                var re = _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return RedirectToAction(nameof(Details), new { id = doctorClinic.DoctorId });
        }

        [HttpGet]
        public IActionResult DetailsHospital(int idD, int idH)
        {
            var model = _context.DoctorHospitals
                .Include(obj => obj.Hospital)
                .Include(obj => obj.BestTimeForVisit)
                .Include(obj => obj.Times)
                .FirstOrDefault(obj => obj.HospitalId == idH && obj.DoctorId == idD);
            return View(model);
        }

        [HttpPost]
        public IActionResult EditHospital([FromBody] DoctorHospital doctorHospital)
        {
            try
            {
                var oldDoctorHospital = _context.DoctorHospitals
                    .Include(obj => obj.BestTimeForVisit)
                    .Include(obj => obj.Times)
                    .AsNoTracking()
                    .FirstOrDefault(obj => obj.HospitalId == doctorHospital.HospitalId && obj.DoctorId == doctorHospital.DoctorId);

                _context.Times.RemoveRange(oldDoctorHospital.Times.Except(doctorHospital.Times));

                oldDoctorHospital.Times = doctorHospital.Times;
                oldDoctorHospital.BestTimeForVisit = doctorHospital.BestTimeForVisit;

                _context.DoctorHospitals.Update(oldDoctorHospital);
                var re = _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return RedirectToAction(nameof(Details), new { id = doctorHospital.DoctorId });
        }

    }
}
