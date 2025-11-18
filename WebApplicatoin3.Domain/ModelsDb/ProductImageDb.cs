using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplicatoin3.Domain.ModelsDb
{
    [Table("product_images")]
    public class ProductImageDb
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("product_id")]
        public Guid ProductId { get; set; }

        [Column("image_url")]
        public string ImageUrl { get; set; }

        [Column("createdat")]
        public DateTime CreatedAt { get; set; }

        // Навигационное свойство
        public ProductDb Product { get; set; }
    }
}
