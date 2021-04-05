using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeisterMask.DataProcessor.ImportDto
{
    public class ImportEmployeeDTO
    {
        public ImportEmployeeDTO()
        {
            Tasks = new List<int>();
        }

        [MinLength(3)]
        [Required]
        [MaxLength(40)]
        [RegularExpression(@"^[A-Za-z0-9]+|[A-Za-z]+$")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{3}-[0-9]{3}-[0-9]{4}$")]
        public string Phone { get; set; }

        public List<int> Tasks { get; set; }


    }
}
