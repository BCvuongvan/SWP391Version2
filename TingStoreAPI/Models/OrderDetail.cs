using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TingStoreAPI.Models
{
    public class OrderDetail
    {
        public int orderId { get; set; }
        [ForeignKey("orderId")]
        public Order order { get; set; }

        public int proId { get; set; }
        [ForeignKey("proId")]
        public Product product { get; set; }

        [Required]
        public int quantity { get; set; }
        [Column(TypeName = "Money")]
        public Decimal subTotal { get; set; }
    }
}