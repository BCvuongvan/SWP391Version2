using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TingStoreAPI.Models
{
    public class ProductImage
    {
        [Key]
        public int imageId { get; set; }
        [Required]
        public int proId { get; set; }
        [ForeignKey("proId")]
        public Product product { get; set; }
        [Required]
        public String imageUrl { get; set; }

        public ProductImage(int proId, String imageUrl)
        {
            this.proId = proId;
            this.imageUrl = imageUrl;
        }
    }
}