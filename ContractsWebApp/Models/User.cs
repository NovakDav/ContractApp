using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;

namespace ContractsWebApp.Models
{
    public class User
    {
        public string Id { get; set; }


        [Required]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string PersonalIdentificationNumber { get; set; }

        [Required]
        [Range(18, 120)]
        public int Age { get; set; }

        [Required]
        public UserRole Role { get; set; }


    }
   
}
