using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PolyclinicProjectKurs.Models;

namespace PolyclinicProjectKurs.Context;

public partial class PolycCursContext : DbContext
{
    public PolycCursContext()
    {
    }

    public PolycCursContext(DbContextOptions<PolycCursContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<Homedoctorcall> Homedoctorcalls { get; set; }

    public virtual DbSet<Medicalrecord> Medicalrecords { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<DoctorAppointmentDates> DoctorAppointmentDates { get; set; }

    public virtual DbSet<DoctorAccount> DoctorAccounts { get; set; }

    public virtual DbSet<DoctorAppointmentWithoutAuthorization> DoctorAppointmentsWithoutAuthorization { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=PolycCurs;Username=postgres;password=1234");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("appointments_pkey");

            entity.ToTable("appointments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AppointmentTime1).HasColumnName("appointment_time");
            entity.Property(e => e.Appointmenttime).HasColumnName("appointmenttime");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.AppointmentStatus).HasColumnType("character varying").HasColumnName("appointmentstatus");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("appointments_doctor_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("appointments_user_id_fkey");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("doctors_pkey");

            entity.ToTable("doctors");

            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.Appointmentinterval).HasColumnName("appointmentinterval");
            entity.Property(e => e.Availabletimeafter).HasColumnName("availabletimeafter").HasColumnType("character varying");
            entity.Property(e => e.Availabletimebefore).HasColumnName("availabletimebefore").HasColumnType("character varying");
            entity.Property(e => e.Doctorname)
                .HasColumnType("character varying")
                .HasColumnName("doctorname");
            entity.Property(e => e.Speciality)
                .HasColumnType("character varying")
                .HasColumnName("speciality");

            entity.HasMany(d => d.DoctorAppointmentDates)
              .WithOne(p => p.Doctor)
              .HasForeignKey(p => p.DoctorId);

            entity.HasMany(d => d.DoctorAccounts)
                .WithOne(p => p.Doctor)
                .HasForeignKey(p => p.DoctorId);

            entity.HasMany(d => d.AppointmentsWithoutAuthorization)
                .WithOne(p => p.Doctor)
                .HasForeignKey(p => p.DoctorId);
        });

        modelBuilder.Entity<DoctorAppointmentWithoutAuthorization>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("appointmentswithoutauthorization");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LastName).HasColumnName("lastname");
            entity.Property(e => e.FirstName).HasColumnName("firstname");
            entity.Property(e => e.Patronymic).HasColumnName("patronymicname");
            entity.Property(e => e.BirthDate).HasColumnName("birthdate");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Phone).HasColumnName("phone");
            entity.Property(e => e.DoctorId).HasColumnName("doctorid");
            entity.Property(e => e.AppointmentDate).HasColumnName("appointmentdate");
            entity.Property(e => e.AppointmentTime).HasColumnName("appointmenttime");

            entity.HasOne(d => d.Doctor)
                  .WithMany(p => p.AppointmentsWithoutAuthorization)
                  .HasForeignKey(d => d.DoctorId)
                  .HasConstraintName("appointmentswithoutauthorization_doctorid_fkey");
        });

        modelBuilder.Entity<DoctorAccount>(entity =>
        {
            entity.HasKey(e => e.DoctorAccountId).HasName("doctoraccounts_pkey");

            entity.ToTable("doctoraccounts");

            entity.Property(e => e.DoctorAccountId).HasColumnName("doctoraccountid");
            entity.Property(e => e.DoctorId).HasColumnName("doctorid");
            entity.Property(e => e.Useremail)
                .HasColumnName("useremail");
            entity.Property(e => e.Userpassword)
                .HasColumnName("userpassword");
            entity.Property(e => e.Phone)
                    .HasColumnName("phone");

            entity.HasOne(d => d.Doctor)
                .WithMany(p => p.DoctorAccounts)
                .HasForeignKey(d => d.DoctorId);
        });

        modelBuilder.Entity<Homedoctorcall>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("homedoctorcalls_pkey");

            entity.ToTable("homedoctorcalls");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Appointmentdate).HasColumnName("appointmentdate");
            entity.Property(e => e.Callreason)
                .HasColumnType("character varying")
                .HasColumnName("callreason");
            entity.Property(e => e.Flatnumber).HasColumnName("flatnumber");
            entity.Property(e => e.Housenumber).HasColumnName("housenumber");
            entity.Property(e => e.Korpus)
                .HasColumnType("character varying")
                .HasColumnName("korpus");
            entity.Property(e => e.LivingPlace)
                .HasColumnType("character varying")
                .HasColumnName("living_place");
            entity.Property(e => e.Street)
                .HasColumnType("character varying")
                .HasColumnName("street");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Homedoctorcalls)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("homedoctorcalls_user_id_fkey");
        });

        modelBuilder.Entity<Medicalrecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("medicalrecords_pkey");

            entity.ToTable("medicalrecords");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.Diagnosis)
                .HasColumnType("character varying")
                .HasColumnName("diagnosis");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Appointment).WithMany(p => p.Medicalrecords)
                .HasForeignKey(d => d.AppointmentId)
                .HasConstraintName("medicalrecords_appointment_id_fkey");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Medicalrecords)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("medicalrecords_doctor_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Medicalrecords)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("medicalrecords_user_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Birthdate).HasColumnName("birthdate");
            entity.Property(e => e.Gender)
                .HasColumnType("character varying")
                .HasColumnName("gender");
            entity.Property(e => e.Snils)
                .HasColumnType("character varying")
                .HasColumnName("snils");
            entity.Property(e => e.Status)
                .HasColumnType("character varying")
                .HasColumnName("status");
            entity.Property(e => e.Useremail)
                .HasColumnType("character varying")
                .HasColumnName("useremail");
            entity.Property(e => e.Username)
                .HasColumnType("character varying")
                .HasColumnName("username");
            entity.Property(e => e.Userpatronymicname)
                .HasColumnType("character varying")
                .HasColumnName("userpatronymicname");
            entity.Property(e => e.Userphone)
                .HasColumnType("character varying")
                .HasColumnName("userphone");
            entity.Property(e => e.Usersurname)
                .HasColumnType("character varying")
                .HasColumnName("usersurname");
            entity.Property(e => e.Userpassword)
               .HasColumnType("character varying")
               .HasColumnName("userpassword");
        });

        modelBuilder.Entity<DoctorAppointmentDates>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("doctorappointmentdates_pkey");

            entity.ToTable("doctorappointmentdates");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.AvailableDate).HasColumnName("availabledate");

            entity.HasOne(d => d.Doctor)
                .WithMany(p => p.DoctorAppointmentDates)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("doctorappointmentdates_doctor_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
