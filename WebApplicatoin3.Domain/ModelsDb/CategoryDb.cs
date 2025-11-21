using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplicatoin3.Domain.ModelsDb
{
    [Table("categories")]
    public class CategoryDb
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("image_url")]
        public string ImageUrl { get; set; }

         [Column("product_count")]  // Обратите внимание - product_count, а не products_count
        public int ProductsCount { get; set; }

        [Column("cratedAt")]  // Исправлено на cratedAt (с опечаткой)
        public DateTime CreatedAt { get; set; }
    }
}
