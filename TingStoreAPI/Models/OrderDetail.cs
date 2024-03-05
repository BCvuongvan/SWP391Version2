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
        public Decimal subTotal { get; set; }

        public OrderDetail(int orderId, int proId, int quantity, decimal subTotal)
        {
            this.orderId = orderId;
            this.proId = proId;
            this.quantity = quantity;
            this.subTotal = subTotal;
        }

        public OrderDetail()
        {

        }

    }
}