using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TingStoreClient.Models
{
    public class ProductImage
    {
        [Key]
        [DisplayName("ID")]
        public int imageId { get; set; }

        [Required(ErrorMessage = ("Please enter product id!"))]
        [DisplayName("Product ID")]
        public int proId { get; set; }
        [ForeignKey("proId")]
        public Product product { get; set; }

        [Required(ErrorMessage = ("Please enter image URL!"))]
        [DisplayName("URL")]
        public String imageUrl { get; set; }

        public bool imageStatus { get; set; }
    }
}