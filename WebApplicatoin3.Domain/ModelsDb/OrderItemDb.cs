using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplicatoin3.Domain.ModelsDb
{
    [Table("order_items")]
    public class OrderItemDb
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("order_id")]
        public Guid OrderId { get; set; }

        [Column("product_id")]
        public Guid ProductId { get; set; }

        [Column("product_name")]
        public string ProductName { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        // Навигационные свойства
        public OrderDb Order { get; set; }
        public ProductDb Product { get; set; }
    }
}
