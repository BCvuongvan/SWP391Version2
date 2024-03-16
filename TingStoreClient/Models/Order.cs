using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TingStoreClient.Models
{
    public class Order
    {
        [Key]
        [DisplayName("ID")]
        public int orderId { get; set; }

        [Required(ErrorMessage = "Please enter Username!")]
        [DisplayName("Username")]
        public String userName { get; set; }

        [ForeignKey("userName")]
        public User user { get; set; }

        [DisplayName("Date")]
        public DateTime orderDate { get; set; }

        [DisplayName("Total Amount")]
        public Decimal TotalAmount { get; set; }

        [DisplayName("Status ID")]
        public int orderStatusId { get; set; }

        [ForeignKey("orderStatusId")]
        public OrderStatus orderStatus { get; set; }

        public ICollection<OrderDetail> orderDetails { get; set; }

        public Order(String userName, DateTime orderDate, Decimal totalAmount, int orderStatusId)
        {
            this.userName = userName;
            this.orderDate = orderDate;
            this.TotalAmount = totalAmount;
            this.orderStatusId = orderStatusId;
        }

        public Order()
        {
            
        }
    }
}