using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplicatoin3.Domain.ModelsDb
{
    [Table("requests")]
    public class RequestDb
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("image_url")]
        public string ImageUrl { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("createdat")]
        public DateTime CreatedAt { get; set; }

        // Навигационное свойство
        public UserDb User { get; set; }
    }
}
