using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TingStoreClient.Models
{
    public class Feedback
    {
        [DisplayName("Username")]
        public String userName { get; set; }
        [ForeignKey("userName")]
        public User user { get; set; }

        [DisplayName("Product ID")]
        public int proId { get; set; }
        [ForeignKey("proId")]
        public Product product { get; set; }

        [Required(ErrorMessage = ("Please enter comment!"))]
        [DisplayName("Comment")]
        public String comment { get; set; }

        [Required(ErrorMessage = ("Please vote rating!"))]
        [DisplayName("Rate")]
        [Range(0, 5, ErrorMessage = "rating  must be between 0 and 5.")]
        public int rating { get; set; }
    }
}