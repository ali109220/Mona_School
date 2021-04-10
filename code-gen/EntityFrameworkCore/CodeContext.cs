
using Core.SharedDomain.Security;
using Domain.Constants;
using Domain.Entities;
using Domain.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore
{ 
    public class CodeContext : IdentityDbContext<User>
    {
        public CodeContext(DbContextOptions<CodeContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ProductType>()
             .HasOne(x => x.Brand);
            builder.Entity<ProductType>()
             .HasOne(x => x.LicenceType);
            builder.Entity<ProductType>()
             .HasOne(x => x.Platform);
            builder.Entity<Customer>()
            .HasOne(x => x.User);
            builder.Entity<RedeemCode>()
            .HasOne(x => x.ProductType)
             .WithMany(x => x.Codes);
            builder.Entity<RedeemCode>()
            .HasOne(x => x.Customer)
             .WithMany(x => x.Codes);
            //builder.Entity<ResumeSpecialty>()
            // .HasOne(x => x.Specialty)
            // .WithOne()
            // .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<PhoneRegionCountry>()
            // .HasOne(x => x.Country)
            // .WithOne()
            // .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<Phone>()
            // .HasOne(x => x.PhoneRegionCountry)
            // .WithOne()
            // .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<Reference>()
            // .HasOne(x => x.JobTitle)
            // .WithOne()
            // .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<Reference>()
            // .HasOne(x => x.Phone)
            // .WithOne()
            // .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<OtherNationality>()
            //  .HasOne(x => x.Country)
            //  .WithOne()
            //  .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<Experience>()
            //  .HasOne(x => x.Country)
            //  .WithOne()
            //  .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<Experience>()
            //  .HasOne(x => x.Sector)
            //  .WithOne()
            //  .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<Experience>()
            //  .HasOne(x => x.Currency)
            //  .WithOne()
            //  .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<Education>()
            //  .HasOne(x => x.Country)
            //  .WithOne()
            //  .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<Education>()
            //  .HasOne(x => x.CertificateFile)
            //  .WithOne()
            //  .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<DrivingLicense>()
            //  .HasOne(x => x.IssuedFrom)
            //  .WithOne()
            //  .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<Certificate>()
            //  .HasOne(x => x.CertificateFile)
            //  .WithOne()
            //  .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<ResumeLanguage>()
            //  .HasOne(x => x.Language)
            //  .WithOne()
            //  .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<JobLanguage>()
            //  .HasOne(x => x.Language)
            //  .WithOne()
            //  .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<JobExperience>()
            //  .HasOne(x => x.JobTitle)
            //  .WithOne()
            //  .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<JobExperience>()
            //  .HasOne(x => x.Category)
            //  .WithOne()
            //  .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<Experience>()
            //  .HasOne(x => x.JobTitle)
            //  .WithOne()
            //  .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<Experience>()
            //  .HasOne(x => x.Category)
            //  .WithOne()
            //  .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<Education>()
            //  .HasOne(x => x.Major)
            //  .WithOne()
            //  .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<JobEducation>()
            //  .HasOne(x => x.Major)
            //  .WithOne()
            //  .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<Company>()
            //  .HasOne(x => x.City)
            //  .WithOne()
            //  .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<Company>()
            //  .HasOne(x => x.Country)
            //  .WithOne()
            //  .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<Job>()
            //  .HasOne(x=>x.City)
            //  .WithOne()
            //  .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<Job>()
            //  .HasOne(x => x.Country)
            //  .WithOne()
            //  .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<Nationality>()
            //  .HasOne(x => x.Country)
            //  .WithOne()
            //  .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<Resume>()
            //  .HasOne(x => x.ResidenceCountry)
            //  .WithOne()
            //  .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<Resume>()
            //  .HasOne(x => x.Nationality)
            //  .WithOne()
            //  .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<Resume>()
            //  .HasOne(x => x.City)
            //  .WithOne()
            //  .OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<Resume>()
            //  .HasMany(c => c.Specialties)
            //  .WithOne(e => e.Resume);
            //builder.Entity<Resume>()
            //  .HasMany(c => c.Skills)
            //  .WithOne(e => e.Resume);
            //builder.Entity<Resume>()
            //  .HasMany(c => c.References)
            //  .WithOne(e => e.Resume);
            //builder.Entity<Country>()
            //  .HasOne(c => c.CreatedUser)
            //  .WithOne();
            //builder.Entity<Job>()
            //  .HasMany(c => c.Compensations)
            //  .WithOne(e => e.Job);


            //builder.Entity<Employee>()
            //  .HasMany(c => c.Notes)
            //  .WithOne(e => e.Employee);

            //// Customize the ASP.NET Core Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Core Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
        //constants 
        public DbSet<RedeemCode> RedeemCodes { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<LicenceType> LicenceTypes { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Question> Questions { get; set; }

        //Job 


        //Resume


        //User
        //public DbSet<User> Users { get; set; }



    }
}
