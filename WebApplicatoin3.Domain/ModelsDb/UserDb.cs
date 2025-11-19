using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicatoin3.Domain.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace WebApplicatoin3.Domain.ModelsDb
{
    [Table("users")]
    public class UserDb
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("login")]
        public string Login { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("role")]
        public Role Role { get; set; }

        [Column("pathImage")]
        public string pathImage { get; set; }

        [Column("createdAt", TypeName = "timestamp")]
        public DateTime CreatedAt { get; set; }

    }
}
