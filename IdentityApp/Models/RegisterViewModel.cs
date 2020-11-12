using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApp.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Region")]
        public byte Region { get; set; }

        public string Password { get; set; }
        public string GenerateNewPassword()
        {
            string allowed = "0123456789abcdefghijklmnopqrstuvwxyz";
            Password = new string(allowed
                .OrderBy(o => Guid.NewGuid())
                .Take(6)
                .ToArray());
            return Password;
        }
    }
}
