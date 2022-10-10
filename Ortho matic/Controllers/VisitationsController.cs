using ClosedXML.Excel;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ortho_matic.Data;
using Ortho_matic.Models;
using Ortho_matic.Models.APIModels;
using Ortho_matic.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ortho_matic.Controllers
{
    [Authorize]
    [EnableCors]
    public class VisitationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public VisitationsController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> IndexAsync()
        {
            ViewBag.Users = await _userManager.GetUsersInRoleAsync("Staff");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVisits()
        {
            return Json(new
            {
                data = await _context.Visitations.Select(obj => new
                {
                    user = obj.User.UserName,
                    doctor = obj.Doctor.Name,
                    date = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(obj.TimeOfVisit, "Egypt Standard Time").ToString("yyyy-MM-dd hh:mm tt"),
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
                    UserName = obj.User.UserName,
                    comment = obj.comment,
                    Latitude = obj.Latitude,
                    Longitude = obj.Longitude,
                    DoctorName = obj.Doctor.Name,
                    TimeOfVisit = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(obj.TimeOfVisit, "Egypt Standard Time").ToString("yyyy-MM-dd hh:mm tt"),
                    Type = obj.ClinicId == null ? "Hospital" : "Clinic",
                    DoctorDegree = obj.Doctor.DoctorDegree.ToString(),
                    DoctorSpecialty = obj.Doctor.DoctorSpecialty.ToString().Replace('_', ' '),
                    HospitalName = obj.HospitalId == null ? null : obj.Hospital.Name,
                    HospitalAddress = obj.HospitalId == null ? null : obj.Hospital.Address,
                    HospitalPhone1 = obj.HospitalId == null ? null : obj.Hospital.Phone1,
                    ClinicAddress = obj.ClinicId == null ? null : obj.Clinic.Address,
                    ClinicPhone1 = obj.ClinicId == null ? null : obj.Clinic.Phone1,
                })
                .FirstOrDefaultAsync(m => m.Id == id);

            if (task == null)
            {
                return RedirectToAction(nameof(IndexAsync));
            }

            return View(task);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetAllTasksOfHospitals()
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

                var hospitals = _context.Hospitals
                    .Where(obj => obj.Region.Name == user.Region.Name && obj.DoctorHospitals.Count != 0)
                    .Select(obj => new
                    {
                        obj.Address,
                        obj.Phone1,
                        obj.Phone2,
                        obj.Phone3,
                        obj.Id,
                        obj.Name,
                        Doctors = obj.DoctorHospitals.Where(obj => obj.LastTimeOfVisitation < lowerB).Select(d => new
                        {
                            obj.Id,
                            DoctorName = d.Doctor.Name,
                            d.Doctor.DoctorSpecialty,
                            d.Doctor.DoctorDegree,
                            d.BestTimeForVisit,
                            d.Times,
                        }),
                    }).ToList();

                var listOfHospitals = new List<HospitalTask>();

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
                    hospitals = listOfHospitals
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetAllTasksOfClinics()
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
                    .Where(obj => obj.Region.Name == user.Region.Name && obj.DoctorClinics.Count != 0)
                    .Select(obj => new
                    {
                        obj.Address,
                        obj.Phone1,
                        obj.Phone2,
                        obj.Phone3,
                        obj.Id,
                        Doctors = obj.DoctorClinics.Where(obj => obj.LastTimeOfVisitation < lowerB).Select(d => new
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

                return Ok(new
                {
                    clinics = listOfClinics,
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

                if (task.ClinicId != null)
                {
                    var clinic = _context.Clinics.Include(obj => obj.DoctorClinics).FirstOrDefault(obj => obj.Id == task.ClinicId);
                    if (clinic.DoctorClinics.FirstOrDefault(obj => obj.DoctorId == task.DoctorId).LastTimeOfVisitation > DateTime.Now.Subtract(TimeSpan.FromHours(48)))
                    {
                        return BadRequest("This task done in the last two days");
                    }
                    clinic.DoctorClinics.FirstOrDefault(obj => obj.DoctorId == task.DoctorId).LastTimeOfVisitation = DateTime.Now;
                    _context.Clinics.Update(clinic);
                }
                else
                {
                    var hospital = _context.Hospitals.Include(obj => obj.DoctorHospitals).FirstOrDefault(obj => obj.Id == task.HospitalId);
                    if (hospital.DoctorHospitals.FirstOrDefault(obj => obj.DoctorId == task.DoctorId).LastTimeOfVisitation > DateTime.Now.Subtract(TimeSpan.FromHours(48)))
                    {
                        return BadRequest("This task done in the last two days");
                    }
                    hospital.DoctorHospitals.FirstOrDefault(obj => obj.DoctorId == task.DoctorId).LastTimeOfVisitation = DateTime.Now;
                    _context.Hospitals.Update(hospital);
                }

                _context.Visitations.Add(visit);
                _context.SaveChanges();

                return Ok("successful operation");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDataToExcelSheet()
        {
            ViewBag.Users = await _userManager.GetUsersInRoleAsync("Staff");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetDataToExcelSheetAsync(ExcelVM model)
        {
            if (ModelState.IsValid)
            {
                var visits = GetVisitsDetail(model);
                return ExportToExcel(visits);
            }
            ViewBag.Users = await _userManager.GetUsersInRoleAsync("Staff");
            return View();
        }

        private IActionResult ExportToExcel(DataTable visits)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Visits");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "UserName";
                worksheet.Cell(currentRow, 2).Value = "TimeOfVisit";
                worksheet.Cell(currentRow, 3).Value = "Address";
                worksheet.Cell(currentRow, 4).Value = "Doctor name";
                worksheet.Cell(currentRow, 5).Value = "Type";
                worksheet.Cell(currentRow, 6).Value = "Comment";

                for (int i = 0; i < visits.Rows.Count; i++)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = visits.Rows[i]["UserName"];
                    worksheet.Cell(currentRow, 2).Value = visits.Rows[i]["TimeOfVisit"];
                    worksheet.Cell(currentRow, 3).Value = visits.Rows[i]["Address"];
                    worksheet.Cell(currentRow, 4).Value = visits.Rows[i]["Doctor name"];
                    worksheet.Cell(currentRow, 5).Value = visits.Rows[i]["Type"];
                    worksheet.Cell(currentRow, 6).Value = visits.Rows[i]["Comment"];
                }

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return File(
                    stream.ToArray(),
                    "application/xlsx",
                    "ExcelFile.xlsx");
            }
        }

        private DataTable GetVisitsDetail(ExcelVM model)
        {
            DataTable dtVisit = new DataTable("VisitDetails");
            dtVisit.Columns.AddRange(new DataColumn[6] { new DataColumn("UserName"),
                                            new DataColumn("TimeOfVisit"),
                                            new DataColumn("Address"),
                                            new DataColumn("Doctor name"),
                                            new DataColumn("Type"),
                                            new DataColumn("Comment"),

            });
            var visits = _context.Visitations
                .Include(obj => obj.User)
                .Include(obj => obj.Doctor)
                .Include(obj => obj.Hospital)
                .Include(obj => obj.Clinic)
                .Where(obj => obj.User.UserName == model.UserName && obj.TimeOfVisit > model.StartTime && obj.TimeOfVisit < model.EndTime)
                .ToList();

            foreach (var visit in visits)
            {
                var time = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(visit.TimeOfVisit, "Egypt Standard Time").ToString("yyyy-MM-dd hh:mm tt");
                dtVisit.Rows.Add(
                    visit.User.UserName,
                    time,
                    visit.HospitalId == null ? visit.Clinic.Address : visit.Hospital.Address,
                    visit.Doctor.Name,
                    visit.HospitalId == null ? "Clinic" : "Hospital",
                    visit.comment
                    );
            }

            return dtVisit;
        }
    }
}
