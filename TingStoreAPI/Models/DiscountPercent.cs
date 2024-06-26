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
        [Required]
        public String discountImage { get; set; }
        public bool isActive { get; set; }
        public DiscountPercent()
        {
        }

        public DiscountPercent(int? proId, int discountPercentage, DateTime startDate, DateTime endDate, String discountImage, bool isActive = true)
        {
            this.proId = proId;
            this.discountPercentage = discountPercentage;
            this.startDate = startDate;
            this.endDate = endDate;
            this.discountImage = discountImage;
            this.isActive = isActive;
        }
        public DiscountPercent(int discountId, int? proId, int discountPercentage, String discountImage, bool isActive = true)
        {
            this.discountId = discountId;
            this.proId = proId;
            this.discountPercentage = discountPercentage;
            this.discountImage = discountImage;
            this.isActive = isActive;
        }
    }
}