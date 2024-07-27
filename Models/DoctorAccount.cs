using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyclinicProjectKurs.Models
{
    public class DoctorAccount
    {
        public int DoctorAccountId { get; set; }
        public int DoctorId { get; set; }
        public string Useremail { get; set; }
        public string Userpassword { get; set; }
        public string Phone { get; set; }

        public Doctor Doctor { get; set; }
    }
}
