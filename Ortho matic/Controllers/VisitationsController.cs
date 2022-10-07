using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Ortho_matic.Data;
using Ortho_matic.Models;
using Ortho_matic.Models.APIModels;
using Ortho_matic.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ortho_matic.Controllers
{
    [Authorize]
    [EnableCors]
    public class VisitationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VisitationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVisits()
        {
            return Json(new
            {
                data = await _context.Visitations.Select(obj => new
                {
                    user = obj.User.EmployeeName,
                    doctor = obj.Doctor.Name,
                    date = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(obj.TimeOfVisit, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt"),
                    type = obj.ClinicId == null ? "Hospital" : "Clinic",
                    obj.Id,
                }).ToListAsync()
            });
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var task = await _context.Visitations
                .Select(obj => new TaskVM
                {
                    Id = obj.Id,
                    UserName = obj.User.EmployeeName,
                    comment = obj.comment,
                    Latitude = obj.Latitude,
                    Longitude = obj.Longitude,
                    DoctorName = obj.Doctor.Name,
                    TimeOfVisit = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(obj.TimeOfVisit, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt"),
                    Type = obj.ClinicId == null ? "Hospital" : "Clinic",
                    DoctorDegree = obj.Doctor.DoctorDegree.ToString(),
                    DoctorSpecialty = obj.Doctor.DoctorSpecialty.ToString(),
                    HospitalName = obj.HospitalId == null ? null : obj.Hospital.Name,
                    HospitalAddress = obj.HospitalId == null ? null : obj.Hospital.Address,
                    HospitalPhone1 = obj.HospitalId == null ? null : obj.Hospital.Phone1,
                    ClinicAddress = obj.ClinicId == null ? null : obj.Clinic.Address,
                    ClinicPhone1 = obj.ClinicId == null ? null : obj.Clinic.Phone1,
                })
                .FirstOrDefaultAsync(m => m.Id == id);

            if (task == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(task);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetAllTasks()
        {
            try
            {
                var claim = User.Claims.FirstOrDefault(obj => obj.Type == "UserName");

                if (claim == null)
                {
                    return BadRequest("Wrong User");
                }

                var user = _context.ApplicationUsers.Include(obj => obj.Region).FirstOrDefault(obj => obj.UserName == claim.Value);

                if (user == null)
                {
                    return BadRequest("Wrong Username");
                }

                var lowerB = DateTime.Now.Subtract(TimeSpan.FromHours(48));

                var clinics = _context.Clinics
                    .Where(obj => obj.Region.Name == user.Region.Name && obj.LastTimeOfVisitation < lowerB)
                    .Select(obj => new
                    {
                        obj.Address,
                        obj.Phone1,
                        obj.Phone2,
                        obj.Phone3,
                        obj.Id,
                        Doctors = obj.DoctorClinics.Select(d => new
                        {
                            obj.Id,
                            DoctorName = d.Doctor.Name,
                            d.Doctor.DoctorSpecialty,
                            d.Doctor.DoctorDegree,
                            d.BestTimeForVisit,
                            d.Times,
                        }),
                    }).ToList();

                var hospitals = _context.Hospitals
                    .Where(obj => obj.Region.Name == user.Region.Name && obj.LastTimeOfVisitation < lowerB)
                    .Select(obj => new
                    {
                        obj.Address,
                        obj.Phone1,
                        obj.Phone2,
                        obj.Phone3,
                        obj.Id,
                        obj.Name,
                        Doctors = obj.DoctorHospitals.Select(d => new
                        {
                            obj.Id,
                            DoctorName = d.Doctor.Name,
                            d.Doctor.DoctorSpecialty,
                            d.Doctor.DoctorDegree,
                            d.BestTimeForVisit,
                            d.Times,
                        }),
                    }).ToList();

                var listOfClinics = new List<ClinicTask>();
                var listOfHospitals = new List<HospitalTask>();

                foreach (var item in clinics)
                {
                    var clinic = new ClinicTask()
                    {
                        ClinicId = item.Id,
                        ClinicAddress = item.Address,
                        ClinicPhone1 = item.Phone1,
                        ClinicPhone2 = item.Phone2,
                        ClinicPhone3 = item.Phone3,
                    };

                    foreach (var doc in item.Doctors)
                    {
                        clinic.DoctorId = doc.Id;
                        clinic.DoctorName = doc.DoctorName;
                        clinic.DoctorDegree = doc.DoctorDegree;
                        clinic.DoctorSpecialty = doc.DoctorSpecialty;
                        clinic.BestTimeForVisit = doc.BestTimeForVisit;
                        clinic.Times = doc.Times;

                        listOfClinics.Add(clinic);
                    }
                }

                foreach (var item in hospitals)
                {
                    var hospital = new HospitalTask()
                    {
                        HospitalId = item.Id,
                        HospitalAddress = item.Address,
                        HospitalPhone1 = item.Phone1,
                        HospitalPhone2 = item.Phone2,
                        HospitalPhone3 = item.Phone3,
                        HospitalName = item.Name
                    };

                    foreach (var doc in item.Doctors)
                    {
                        hospital.DoctorId = doc.Id;
                        hospital.DoctorName = doc.DoctorName;
                        hospital.DoctorDegree = doc.DoctorDegree;
                        hospital.DoctorSpecialty = doc.DoctorSpecialty;
                        hospital.BestTimeForVisit = doc.BestTimeForVisit;
                        hospital.Times = doc.Times;

                        listOfHospitals.Add(hospital);
                    }
                }

                return Ok(new
                {
                    clinics = listOfClinics,
                    hospitals = listOfHospitals
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult DoneVisit([FromBody] VisitTask task)
        {
            try
            {
                var claim = User.Claims.FirstOrDefault(obj => obj.Type == "UserName");

                if (claim == null)
                {
                    return BadRequest("Wrong User");
                }

                var user = _context.ApplicationUsers.Include(obj => obj.Region).FirstOrDefault(obj => obj.UserName == claim.Value);

                if (user == null)
                {
                    return BadRequest("Wrong Username");
                }

                if ((task.ClinicId == null && task.HospitalId == null) || (task.ClinicId != null && task.HospitalId != null))
                {
                    return BadRequest("Wrong must submit clinic or hospital");
                }

                var visit = new Visitation()
                {
                    DoctorId = task.DoctorId,
                    ClinicId = task.ClinicId,
                    HospitalId = task.HospitalId,
                    Latitude = task.Latitude,
                    Longitude = task.Longitude,
                    comment = task.comment,
                    TimeOfVisit = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Egypt Standard Time"),
                    UserId = user.Id,
                };

                _context.Visitations.Add(visit);
                _context.SaveChanges();

                if (task.ClinicId != null)
                {
                    var clinic = _context.Clinics.Find(task.ClinicId);
                    clinic.LastTimeOfVisitation = DateTime.Now;
                    _context.Clinics.Update(clinic);
                }

                if (task.HospitalId == null)
                {
                    var hospital = _context.Hospitals.Find(task.HospitalId);
                    hospital.LastTimeOfVisitation = DateTime.Now;
                    _context.Hospitals.Update(hospital);
                }

                _context.SaveChanges();
                return Ok("successful operation");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
