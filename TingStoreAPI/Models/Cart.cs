using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TingStoreAPI.Models
{
    public class Cart
    {
        public String userName { get; set; }

        [ForeignKey("userName")]
        public User user { get; set; }

        public int proId { get; set; }
        [ForeignKey("proId")]
        public Product product { get; set; }

        [Required]
        public int quantity { get; set; }
    }
}