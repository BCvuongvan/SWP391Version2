using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TingStoreClient.Models
{
    public class Cart
    {
        [DisplayName("Username")]
        public String userName { get; set; }

        [ForeignKey("userName")]
        public User user { get; set; }

        [DisplayName("Product ID")]
        public int proId { get; set; }
        [ForeignKey("proId")]
        public Product product { get; set; }

        [Required(ErrorMessage = "Please enter quantity!")]
        [DisplayName("Quantity")]
        public int quantity { get; set; }
    }
}