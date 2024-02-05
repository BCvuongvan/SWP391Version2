using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TingStoreAPI.Models
{
    public class Feedback
    {
        public String userName { get; set; }
        [ForeignKey("userName")]
        public User user { get; set; }

        public int proId { get; set; }
        [ForeignKey("proId")]
        public Product product { get; set; }

        [Required]
        public String comment { get; set; }
        [Required]
        [Range(0, 5, ErrorMessage = "rating  must be between 0 and 5.")]
        public int rating { get; set; }
    }
}