using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplicatoin3.Domain.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } // Связь с IdentityUser

        // Дополнительные поля
        public string? FullName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
    }
}
