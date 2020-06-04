using System;
using System.Collections.Generic;
using System.Text;
using HRMS_Project.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HRMS_Project.Data
{
    public class ApplicationDbContext : IdentityDbContext<Employee>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<Job> Job { get; set; }
        public virtual DbSet<AvailableAbsence> AvailableAbsence { get; set; }
        public virtual DbSet<AbsenceType> AbsenceType { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>(entity =>
            {


                entity.Property(e => e.IdEmployee).ValueGeneratedOnAdd()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                entity.Property(e => e.BirthDate).HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.IdCardNumber).HasMaxLength(6);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Pesel)
                    .IsRequired()
                    .HasColumnName("PESEL")
                    .HasMaxLength(11);

                entity.Property(e => e.SecondName).HasMaxLength(50);
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.HasKey(e => e.IdJob)
                    .HasName("Job_pk");

                entity.Property(e => e.IdJob).ValueGeneratedOnAdd();

                entity.Property(e => e.JobName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            /*
             *
             **********ABSENCE
             * 
             */

            modelBuilder.Entity<AbsenceType>(entity =>
            {
                entity.HasKey(e => e.IdAbsenceType)
                    .HasName("AbsenceType_pk");

                entity.Property(e => e.IdAbsenceType).ValueGeneratedOnAdd();

                entity.Property(e => e.AbsenceTypeName)
                    .IsRequired()
                    .HasColumnName("AbsenceTypeName");
            });

            modelBuilder.Entity<AvailableAbsence>(entity =>
            {
                entity.HasKey(e => e.IdAvailableAbsence)
                    .HasName("AvailableAbsence_pk");

                entity.Property(e => e.IdAvailableAbsence).ValueGeneratedOnAdd();

                entity.Property(e => e.AvailableDays);

                entity.Property(e => e.IdEmployee)
                    .IsRequired();

                entity.Property(e => e.UsedAbsence).HasDefaultValue(0);

                entity.HasOne(d => d.IdAbsenceTypeNavigation)
                    .WithMany(p => p.AvailableAbsence)
                    .HasForeignKey(d => d.IdAbsenceType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AvailableAbsence_AbsenceType");

                entity.HasOne(d => d.IdEmployeeNavigation)
                    .WithMany(p => p.AvailableAbsence)
                    .HasForeignKey(d => d.IdEmployee)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AvailableAbsence_Emp");
            });

        }

    }
}
