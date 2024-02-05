using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TingStoreAPI.Models
{
    public class DiscountPercent
    {
        [Key]
        public int discountId { get; set; }
        [Required]
        public int? proId { get; set; }
        [ForeignKey("proId")]
        public Product product { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "Discount percentage must be between 0 and 100.")]
        public int discountPercentage { get; set; }
        [Required]
        public DateTime startDate { get; set; }
        [Required]
        public DateTime endDate { get; set; }
        public bool isActive { get; set; }
    }
}