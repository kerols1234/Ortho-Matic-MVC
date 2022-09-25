using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ortho_matic.Models;

namespace Ortho_matic.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<DoctorHospital>().HasKey(dh => new { dh.HospitalId, dh.DoctorId });
            builder.Entity<DoctorClinic>().HasKey(dc => new { dc.ClinicId, dc.DoctorId });
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<DoctorHospital> DoctorHospitals { get; set; }
        public DbSet<DoctorClinic> DoctorClinics { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Time> Times { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Visitation> Visitations { get; set; }
    }
}
