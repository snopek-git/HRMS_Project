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

        public virtual DbSet<Benefit> Benefit { get; set; }
        public virtual DbSet<Contract> Contract { get; set; }
        public virtual DbSet<ContractBenefit> ContractBenefit { get; set; }
        public virtual DbSet<ContractStatus> ContractStatus { get; set; }
        public virtual DbSet<ContractType> ContractType { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<Job> Job { get; set; }

        //        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //        {
        //            if (!optionsBuilder.IsConfigured)
        //            {
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
        //                optionsBuilder.UseSqlServer("Data Source=db-mssql;Initial Catalog=s15885;Integrated Security=True");
        //            }
        //        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Benefit>(entity =>
            {
                entity.HasKey(e => e.IdBenefit)
                    .HasName("Benefit_pk");

                entity.Property(e => e.IdBenefit).ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Price).HasColumnType("decimal(6, 2)");
            });

            modelBuilder.Entity<Contract>(entity =>
            {
                

                entity.Property(e => e.IdContract).ValueGeneratedOnAdd()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                entity.HasKey(e => e.IdContract)
                    .HasName("Contract_pk");

                entity.Property(e => e.ContractNumber);

                entity.Property(e => e.ContractEnd).HasColumnType("date");

                entity.Property(e => e.ContractStart).HasColumnType("date");

                entity.Property(e => e.Salary).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.IdContractStatusNavigation)
                    .WithMany(p => p.Contract)
                    .HasForeignKey(d => d.IdContractStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Contract_ContractStatus");

                entity.HasOne(d => d.IdContractTypeNavigation)
                    .WithMany(p => p.Contract)
                    .HasForeignKey(d => d.IdContractType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Contract_ContractType");

                entity.HasOne(d => d.IdEmployeeNavigation)
                    .WithMany(p => p.Contract)
                    .HasForeignKey(d => d.IdEmployee)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Contract_Employee");
            });

            modelBuilder.Entity<ContractBenefit>(entity =>
            {
                entity.HasKey(e => new { e.IdBenefitContract, e.IdBenefit, e.IdContract })
                    .HasName("ContractBenefit_pk");

                entity.Property(e => e.IdBenefit).HasColumnName("IdBenefit");

                entity.Property(e => e.IdContract).HasColumnName("IdContract");

                entity.Property(e => e.ExpiryDate).HasColumnType("date");

                entity.Property(e => e.StartDate).HasColumnType("date");
            });

            modelBuilder.Entity<ContractStatus>(entity =>
            {
                

                entity.Property(e => e.IdContractStatus).ValueGeneratedOnAdd()
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                entity.HasKey(e => e.IdContractStatus)
                    .HasName("ContractStatus_pk");

                entity.Property(e => e.StatusName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<ContractType>(entity =>
            {
               

                entity.Property(e => e.IdContractType).ValueGeneratedOnAdd()
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                entity.HasKey(e => e.IdContractType)
                    .HasName("ContractType_pk");

                entity.Property(e => e.ContractTypeName)
                    .IsRequired()
                    .HasColumnName("ContractType")
                    .HasMaxLength(100);
            });

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
        }

    }
}
