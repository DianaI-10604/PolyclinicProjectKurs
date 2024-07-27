using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolyclinicProjectKurs.Models;

public partial class Homedoctorcall
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? LivingPlace { get; set; }

    public string? Street { get; set; }

    public int? Housenumber { get; set; }

    public int? Flatnumber { get; set; }

    public string? Korpus { get; set; }

    public DateOnly? Appointmentdate { get; set; }

    public string? Callreason { get; set; }

    public virtual User? User { get; set; }
}
