using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace PolyclinicProjectKurs.Models
{
    public class DoctorAppointmentDates
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DateOnly AvailableDate { get; set; }

        public virtual Doctor Doctor { get; set; }
        
    }
}
