using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TingStoreClient.Models
{
    public class OrderDetail
    {
        [DisplayName("ID")]
        public int orderId { get; set; }
        [ForeignKey("orderId")]
        public Order order { get; set; }

        [DisplayName("Product ID")]
        public int proId { get; set; }
        [ForeignKey("proId")]
        public Product product { get; set; }

        [Required(ErrorMessage = ("Please enter quantity!"))]
        [DisplayName("Quantity")]
        public int quantity { get; set; }

        [DisplayName("Sub Total")]
        [Column(TypeName = "Money")]
        public long subTotal { get; set; }
    }
}