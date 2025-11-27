using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplicatoin3.Domain.ModelsDb
{
    [Table("products")]
    public class ProductDb
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("category_id")]
        public Guid CategoryId { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("old_price")]
        public decimal? OldPrice { get; set; }

        [Column("material")]
        public string? Material { get; set; }

        [Column("dimensions")]
        public string? Dimensions { get; set; }

        [Column("color")]
        public string? Color { get; set; }

        [Column("image_url")]
        public string? ImageUrl { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        // Навигационные свойства
        public CategoryDb Category { get; set; }

        // Добавляем коллекцию изображений
        public List<ProductImageDb> ProductImages { get; set; } = new List<ProductImageDb>();
    }
}
