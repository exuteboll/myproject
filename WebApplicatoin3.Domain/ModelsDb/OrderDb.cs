using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicatoin3.Domain.Enum;

namespace WebApplicatoin3.Domain.ModelsDb
{
    [Table("orders")]
    public class OrderDb
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("product_id")]
        public Guid ProductId { get; set; }

        [Column("product_name")]
        public string ProductName { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("createdat")]
        public DateTime CreatedAt { get; set; }

        // Навигационные свойства
        public UserDb User { get; set; }
        public ProductDb Product { get; set; }
        public List<OrderItemDb> OrderItems { get; set; } = new List<OrderItemDb>();
    }
}
