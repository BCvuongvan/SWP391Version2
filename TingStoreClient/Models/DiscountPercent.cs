using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TingStoreClient.Models
{
    public class DiscountPercent
    {
        [Key]
        [DisplayName("ID")]
        public int discountId { get; set; }

        [Required(ErrorMessage = ("Please enter product Id!"))]
        [DisplayName("Product ID")]
        public int? proId { get; set; }
        [ForeignKey("proId")]
        public Product product { get; set; }

        [Required(ErrorMessage = ("Please enter discount percent!"))]
        [DisplayName("Discount Percent")]
        [Range(0, 100, ErrorMessage = "Discount percentage must be between 0 and 100.")]
        public int discountPercentage { get; set; }

        [Required(ErrorMessage = ("Please enter date start!"))]
        [DisplayName("Date Start")]
        public DateTime startDate { get; set; }

        [Required(ErrorMessage = ("Please enter date end!"))]
        [DisplayName("Date End")]
        public DateTime endDate { get; set; }
        [Required]
        public String discountImage { get; set; }
        [DisplayName("Active")]
        public bool isActive { get; set; }
    }
}