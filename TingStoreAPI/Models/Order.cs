using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TingStoreAPI.Models
{
    public class Order
    {

        [Key]
        public int orderId { get; set; }
        [Required]
        public String userName { get; set; }

        [ForeignKey("userName")]
        public User user { get; set; }

        public DateTime orderDate { get; set; }
        [Column(TypeName = "Money")]

        public decimal TotalAmount { get; set; }

        public int orderStatusId { get; set; }

        [ForeignKey("orderStatusId")]
        public OrderStatus orderStatus { get; set; }

        public ICollection<OrderDetail> orderDetails { get; set; }

        public Order(int orderId, String userName, DateTime orderDate, decimal totalAmount, int orderStatusId)
        {
            this.orderId = orderId;
            this.userName = userName;
            this.orderDate = orderDate;
            this.TotalAmount = totalAmount;
            this.orderStatusId = orderStatusId;
        }
        public Order()
        {

        }
        public Order(String userName, DateTime orderDate, decimal totalAmount, int orderStatusId)
        {
            this.userName = userName;
            this.orderDate = orderDate;
            this.TotalAmount = totalAmount;
            this.orderStatusId = orderStatusId;
        }
    }

}