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
    public class VisitationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VisitationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllTasks()
        {
            try
            {
                var claim = User.Claims.FirstOrDefault(obj => obj.Type == "UserName");

                if (claim == null)
                {
                    return Ok(new { success = false, message = "Wrong User" });
                }

                var user = _context.ApplicationUsers.Include(obj => obj.Region).FirstOrDefault(obj => obj.UserName == claim.Value);

                if (user == null)
                {
                    return Ok(new { success = false, message = "Wrong Username" });
                }

                var doneTasks = _context.Visitations.Where(obj => obj.UserId == user.Id
                                                     && DateTime.Now.Subtract(obj.TimeOfVisit) < TimeSpan.FromHours(48)
                                                     && DateTime.Now.Subtract(obj.TimeOfVisit) > TimeSpan.FromHours(0));

                var clinics = _context.Clinics
                    .Where(obj => obj.Region.Name == user.Region.Name && !doneTasks.Any(t => t.ClinicId == obj.Id))
                    .Select(obj => new
                    {
                        obj.Latitude,
                        obj.Longitude,
                        obj.Address,
                        obj.PhoneNumber,
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
                    .Where(obj => obj.Region.Name == user.Region.Name && !doneTasks.Any(t => t.HospitalId == obj.Id))
                    .Select(obj => new
                    {
                        obj.Latitude,
                        obj.Longitude,
                        obj.Address,
                        obj.PhoneNumber,
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

                return Ok(new
                {
                    success = true,
                    message = "successful operation",
                    data = new { clinics, hospitals }
                });
            }
            catch (Exception e)
            {
                return Ok(new
                {
                    success = false,
                    message = e.Message,
                });
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DoneVisit([FromBody] VisitTask task)
        {
            try
            {
                var claim = User.Claims.FirstOrDefault(obj => obj.Type == "UserName");

                if (claim == null)
                {
                    return Ok(new { success = false, message = "Wrong User" });
                }

                var user = _context.ApplicationUsers.Include(obj => obj.Region).FirstOrDefault(obj => obj.UserName == claim.Value);

                if (user == null)
                {
                    return Ok(new { success = false, message = "Wrong Username" });
                }

                if ((task.ClinicId == null && task.HospitalId == null) || (task.ClinicId != null && task.HospitalId != null))
                {
                    return Ok(new { success = false, message = "Wrong must submit clinic or hospital" });
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

                return Ok(new
                {
                    success = true,
                    message = "successful operation",
                });
            }
            catch (Exception e)
            {
                return Ok(new
                {
                    success = false,
                    message = e.Message,
                });
            }
        }
    }
}
