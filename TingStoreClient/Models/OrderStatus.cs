using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TingStoreClient.Models
{
    public class OrderStatus
    {
        [Key]
        [DisplayName("Status ID")]
        public int orderStatusId { get; set; }

        [Required(ErrorMessage = "Please enter status name!")]
        [DisplayName("Status Name")]
        [MaxLength(100)]
        public String statusName { get; set; }

        public ICollection<Order> orders { get; set; }
    }
}