using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TingStoreAPI.Models
{
    public class OrderStatus
    {
        [Key]
        public int orderStatusId { get; set; }
        [Required]
        [MaxLength(100)]
        public String statusName { get; set; }

        public ICollection<Order> orders { get; set; }
    }
}