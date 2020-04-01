using System;

namespace cw5.Models
{
    public class Enroll
    {
        public int IdEnrollment { get; set; }
        public int Semester { get; set; }
        public DateTime StartDate { get; set; }
        public Study Study { get; set; }
    }
}
