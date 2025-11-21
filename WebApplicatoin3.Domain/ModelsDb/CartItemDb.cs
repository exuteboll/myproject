using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplicatoin3.Domain.ModelsDb
{
    [Table("cart_items")]
    public class CartItemDb
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("product_id")]
        public Guid ProductId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Навигационные свойства
        public UserDb User { get; set; }
        public ProductDb Product { get; set; }
    }
}
