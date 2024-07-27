using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolyclinicProjectKurs.Models;

public partial class Appointment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? DoctorId { get; set; }

    public DateOnly? Appointmenttime { get; set; }

    public TimeOnly? AppointmentTime1 { get; set; }

    public string? AppointmentStatus { get; set; }

    public virtual Doctor? Doctor { get; set; }

    public virtual ICollection<Medicalrecord> Medicalrecords { get; set; } 

    public virtual User? User { get; set; }
}
