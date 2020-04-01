using System;
using System.ComponentModel.DataAnnotations;

namespace cw5.Models
{
    public class Enrollv2
    {
        [Required]
        public string IndexNumber { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        [Required]
        public string Studies { get; set; }
    }
}
