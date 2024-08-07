using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolyclinicProjectKurs.Models;

public partial class Doctor
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int DoctorId { get; set; }

    public string? Doctorname { get; set; }

    public string? Speciality { get; set; }

    public string? Availabletimebefore { get; set; }

    public string? Availabletimeafter { get; set; }

    public int? Appointmentinterval { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Medicalrecord> Medicalrecords { get; set; } = new List<Medicalrecord>();

    public virtual ICollection<DoctorAppointmentDates> DoctorAppointmentDates { get; set; }
    public ICollection<DoctorAccount> DoctorAccounts { get; set; }
    public ICollection<DoctorAppointmentWithoutAuthorization> AppointmentsWithoutAuthorization { get; set; }
}
