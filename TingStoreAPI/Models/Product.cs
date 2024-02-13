using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TingStoreAPI.Models
{
    public class Product
    {
        [Key]
        public int proId { get; set; }
        [Required]
        public String proName { get; set; }
        [Required]
        public String proDescription { get; set; }
        [Required]
        public decimal proPrice { set; get; }
        [Required]
        public int proQuantity { get; set; }
        [Required]
        public String proImage { get; set; }
        public bool proStatus { get; set; }
        [Required]
        public int cateId { get; set; }
        [ForeignKey("cateId")]
        public Category category { get; set; }

        public ICollection<ProductImage> productImages { get; set; }
        public ICollection<DiscountPercent> discountPercents { get; set; }
        public ICollection<Feedback> feedbacks { get; set; }
        public ICollection<OrderDetail> orderDetails { get; set; }
        public ICollection<Cart> carts { get; set; }
        public Product(string proName, string proDescription, decimal proPrice, int proQuantity, string proImage, int cateId, bool proStatus = true)
        {
            this.proName = proName;
            this.proDescription = proDescription;
            this.proPrice = proPrice;
            this.proQuantity = proQuantity;
            this.proImage = proImage;
            this.cateId = cateId;
            this.proStatus = proStatus;
        }
        public Product(int proId, string proName, string proDescription, decimal proPrice, int proQuantity, string proImage, int cateId, bool proStatus = true)
        {
            this.proId = proId;
            this.proName = proName;
            this.proDescription = proDescription;
            this.proPrice = proPrice;
            this.proQuantity = proQuantity;
            this.proImage = proImage;
            this.cateId = cateId;
            this.proStatus = proStatus;
        }

        public Product()
        {
        }
    }
}