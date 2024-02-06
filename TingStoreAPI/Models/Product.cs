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
        [Column(TypeName = "Money")]          // ánh xạ thuộc tính Price, kiểu decimal vào cột Price,  kiểu Money của bảng
        public long proPrice { set; get; }
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
    }
}